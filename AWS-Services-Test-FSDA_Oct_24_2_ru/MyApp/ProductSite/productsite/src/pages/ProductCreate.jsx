import {useState} from "react";
import {useNavigate} from "react-router-dom";
import ProductForm from "../components/ProductForm.jsx";
import api from "../api.js";


const initialForm={
    name:"",
    description:"",
    price:"",
    category:""
}

export default function ProductCreate(){
    const [form,setForm]=useState(initialForm)
    const [imageFile,setImageFile]=useState(null)
    const [loading,setLoading]=useState(false)
    const [error,setError]=useState("")
    const navigate = useNavigate();

    const onChange=(e)=>{
        setForm((prev)=>({...prev,[e.target.name]:e.target.value}))
    }

    const onImageChange=(e)=>{
        setImageFile(e.target.files?.[0]??null)
    }

    const onSubmit=async (e)=>{
        e.preventDefault()
        setLoading(true)
        setError("")
        try {
            const formData = new FormData()
            formData.append("name",form.name)
            formData.append("description",form.description)
            formData.append("price",form.price)
            formData.append("category",form.category)
            if(imageFile){
                formData.append("image", imageFile)
            }
            await api.post("/products",formData,
                {
                    headers:{"Content-Type":"multipart/form-data"}
                });
            navigate("/products")
        }
        catch{
            setError("Failed to create product")
        }
        finally{
            setLoading(false)
        }
    }

    return (
        <div>
            <h2 className="h4 mb-3">Product Create</h2>
            <ProductForm
                form={form}
                imageFile={imageFile}
                onChange={onChange}
                onImageChange={onImageChange}
                onSubmit={onSubmit}
                loading={loading}
                submitText="Create product"
                error={error}
            />
        </div>
    );
}