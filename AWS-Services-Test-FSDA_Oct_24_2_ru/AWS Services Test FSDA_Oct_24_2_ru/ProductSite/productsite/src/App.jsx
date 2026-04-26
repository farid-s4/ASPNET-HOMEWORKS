import {Link,Navigate, Routes, Route} from 'react-router-dom'
import ProductList from "./pages/ProductList";
import ProductCreate from "./pages/ProductCreate";
import ProductEdit from "./pages/ProductEdit";
import ProductDetails from "./pages/ProductDetails";

export default function App(){
    return (
        <div className="container py-4">
            <header className="d-flex justify-content-between align-items-center mb-4">
                <h1 className="h3 m-0">Product Management</h1>
                <Link to="/products"
                      className="btn btn-outline-primary">
                    Products
                </Link>
            </header>
            <Routes>
                <Route path="/" element={<Navigate to="/products" replace />} />
                <Route path="/products" element={<ProductList/>} />
                <Route path="/products/create" element={<ProductCreate/>} />
                <Route path="/products/edit/:id" element={<ProductEdit/>} />
                <Route path="/products/:id" element={<ProductDetails/>} />
            </Routes>
        </div>
    )
}