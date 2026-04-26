import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import api from "../api";
import ProductForm from "../components/ProductForm";

export default function ProductEdit() {
    const { id } = useParams();
    const navigate = useNavigate();

    const [form, setForm] = useState({
        name: "",
        description: "",
        price: "",
        category: ""
    });
    const [imageFile, setImageFile] = useState(null);
    const [loading, setLoading] = useState(false);
    const [initialLoading, setInitialLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        const fetchProduct = async () => {
            setInitialLoading(true);
            setError("");
            try {
                const response = await api.get(`/products/${id}`);
                const p = response.data;
                setForm({
                    name: p.name || "",
                    description: p.description || "",
                    price: p.price || "",
                    category: p.category || ""
                });
            } catch {
                setError("Failed to load product.");
            } finally {
                setInitialLoading(false);
            }
        };

        fetchProduct();
    }, [id]);

    const onChange = (e) => {
        setForm((prev) => ({ ...prev, [e.target.name]: e.target.value }));
    };

    const onImageChange = (e) => {
        setImageFile(e.target.files?.[0] ?? null);
    };

    const onSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError("");
        try {
            const formData = new FormData();
            formData.append("name", form.name);
            formData.append("description", form.description);
            formData.append("price", form.price);
            formData.append("category", form.category);
            if (imageFile) {
                formData.append("image", imageFile);
            }

            await api.put(`/products/${id}`, formData, {
                headers: { "Content-Type": "multipart/form-data" }
            });

            navigate("/products");
        } catch {
            setError("Failed to update product.");
        } finally {
            setLoading(false);
        }
    };

    if (initialLoading) {
        return <p>Loading product...</p>;
    }

    return (
        <div>
            <h2 className="h4 mb-3">Edit Product</h2>
            <ProductForm
                form={form}
                imageFile={imageFile}
                onChange={onChange}
                onImageChange={onImageChange}
                onSubmit={onSubmit}
                loading={loading}
                submitText="Save Changes"
                error={error}
            />
        </div>
    );
}

