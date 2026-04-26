import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import api from "../api";
import { PLACEHOLDER_IMAGE } from "../constants";

export default function ProductDetails() {
    const { id } = useParams();
    const [product, setProduct] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        const fetchProduct = async () => {
            setLoading(true);
            setError("");
            try {
                const response = await api.get(`/products/${id}`);
                setProduct(response.data);
            } catch {
                setError("Failed to load product details.");
            } finally {
                setLoading(false);
            }
        };

        fetchProduct();
    }, [id]);

    if (loading) {
        return <p>Loading product details...</p>;
    }

    if (error) {
        return <div className="alert alert-danger">{error}</div>;
    }

    if (!product) {
        return <div className="alert alert-warning">Product not found.</div>;
    }

    const discountWindow =
        product.discountStart && product.discountEnd ?
            `${new Date(product.discountStart).toLocaleString()} - ${new Date(product.discountEnd).toLocaleString()}`
            : "Not Configured";

    return (
        <div className="card shadow-sm p-4">
            <img
                src={product.imageUrl || PLACEHOLDER_IMAGE}
                alt={product.name}
                className="details-image mb-3"
            />
            <h2 className="h4">{product.name}</h2>
            <p className="mb-2">
                <strong>Category:</strong> {product.category}
            </p>
            <p className="mb-2">
                <strong>Price:</strong> ${Number(product.price).toFixed(2)}
            </p>
            <p className="mb-2">
                <strong>Created:</strong> {new Date(product.createdAt).toLocaleString()}
            </p>
            <p className="mb-2">
                <strong>Discount window:</strong> {discountWindow}
            </p>
            <p className="mb-3">{product.description}</p>

            <div className="d-flex gap-2">
                <Link to={`/products/edit/${product.id}`} className="btn btn-primary">
                    Edit
                </Link>
                <Link to="/products" className="btn btn-outline-secondary">
                    Back
                </Link>
            </div>
        </div>
    );
}
