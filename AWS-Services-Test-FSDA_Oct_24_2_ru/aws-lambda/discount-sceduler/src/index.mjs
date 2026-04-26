import {getPool, sql} from "./db.mjs";

const ACTIVATE_DISCOUNT_SQL = `
UPDATE p
SET p.IsDiscountActive = 1, p.CurrentPrice = p.BasePrice * 0.7
FROM Products p
WHERE p.IsDiscountActive = 0 
AND p.DiscountStart IS NOT NULL
AND p.DiscountStart <= @nowUtc
AND p.DiscountEnd IS NOT NULL
AND p.DiscountEnd > @nowUtc
`

const DEACTIVATE_DISCOUNT_SQL = `
    UPDATE p
    SET p.IsDiscountActive = 0, p.CurrentPrice = p.BasePrice
    FROM Products p
    WHERE p.IsDiscountActive = 1
      AND (
        p.DiscountStart IS NULL
       OR p.DiscountStart
        > @nowUtc
       OR p.DiscountEnd IS NULL
       OR p.DiscountEnd <= @nowUtc
        )
`

async function runUpdate(pool, sqlText, nowUtc){
    const request = pool.request()

    request.input("nowUtc", sql.DateTime2, nowUtc)

    const result = await request.query(sqlText)
    return result.rowsAffected?.[0]??0;
}

export const handler = async (event) => {
    const startedAt = new Date()
    console.info("Discount scheduler started:" ,
        {event,
            startedAtUtc: startedAt.toISOString()});

    const pool = await getPool();

    try {
        const activatedCount = await runUpdate(pool, ACTIVATE_DISCOUNT_SQL, startedAt);
        const deactivatedCount = await runUpdate(pool, DEACTIVATE_DISCOUNT_SQL, startedAt);

        const message = {
            status:"ok",
            startedAtUtc: startedAt.toISOString(),
            activatedCount,
            deactivatedCount
        }
        console.info("Discount scheduler finished",message);
        return {
            statusCode:200,
            body: JSON.stringify(message),
        }
    }
    catch (error) {
        console.error("Discount scheduler failed",{
            message: error.message,
            stack: error.stack,
            code: error.code,
            name: error.name,
        });
        return {
            statusCode:500,
            body: JSON.stringify({
                status:"error",
                errorMessage: error.message,
            }),
        }
    }
}
