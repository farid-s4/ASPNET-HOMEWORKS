import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import api from "../api";
import { PLACEHOLDER_IMAGE } from "../constants";

export default function ProductList() {
    const [products, setProducts] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    const loadProducts = async () => {
        setLoading(true);
        setError("");
        try {
            const response = await api.get("/products");
            setProducts(response.data);
        } catch {
            setError("Failed to load products.");
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadProducts();
    }, []);

    const deleteProduct = async (id) => {
        if (!window.confirm("Delete this product?")) {
            return;
        }
        try {
            await api.delete(`/products/${id}`);
            await loadProducts();
        } catch {
            setError("Failed to delete product.");
        }
    };

    if (loading) {
        return <p>Loading products...</p>;
    }

    return (
        <div>
            <div className="d-flex justify-content-between align-items-center mb-3">
                <h2 className="h4 m-0">Products</h2>
                <Link to="/products/create" className="btn btn-primary">
                    + Add Product
                </Link>
            </div>

            {error ? <div className="alert alert-danger">{error}</div> : null}

            <div className="row g-3">
                {products.map((product) => (
                    <div key={product.id} className="col-md-6 col-lg-4">
                        <div className="card h-100 shadow-sm">
                            <img
                                src={product.imageUrl || PLACEHOLDER_IMAGE}
                                className="card-img-top product-image"
                                alt={product.name}
                            />
                            <div className="card-body">
                                <h5 className="card-title">{product.name}</h5>
                                <p className="card-text mb-1">
                                    <strong>Category:</strong> {product.category}
                                </p>
                                <p className="card-text mb-3">
                                    <strong>Price:</strong> ${Number(product.price).toFixed(2)}
                                </p>
                                <div className="d-flex gap-2 flex-wrap">
                                    <Link to={`/products/${product.id}`} className="btn btn-sm btn-outline-secondary">
                                        Details
                                    </Link>
                                    <Link to={`/products/edit/${product.id}`} className="btn btn-sm btn-outline-primary">
                                        Edit
                                    </Link>
                                    <button
                                        type="button"
                                        className="btn btn-sm btn-outline-danger"
                                        onClick={() => deleteProduct(product.id)}
                                    >
                                        Delete
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
}

