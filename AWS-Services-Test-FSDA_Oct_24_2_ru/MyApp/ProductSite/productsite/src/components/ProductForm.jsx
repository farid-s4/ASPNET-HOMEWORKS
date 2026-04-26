export default function ProductForm({
    form,
    imageFile,
    onChange,
    onImageChange,
    onSubmit,
    loading,
    submitText,
    error
}) {
    return (
        <form onSubmit={onSubmit} className="card shadow-sm p-4">
            <div className="mb-3">
                <label className="form-label">Name</label>
                <input
                    className="form-control"
                    type="text"
                    name="name"
                    value={form.name}
                    onChange={onChange}
                    maxLength={100}
                    required/>
            </div>
            <div className="mb-3">
                <label className="form-label">Description</label>
                <input
                    className="form-control"
                    type="text"
                    name="description"
                    value={form.description}
                    onChange={onChange}
                    maxLength={1000}
                    required/>
            </div>
            <div className="row">
                <label className="form-label">Price</label>
                <input
                    className="form-control"
                    type="number"
                    name="price"
                    value={form.price}
                    onChange={onChange}
                    min="0.01"
                    step="0.01"
                    required/>
            </div>
            <div className="col-md-6 mb-3">
                <label className="form-label">Category</label>
                <input
                    className="form-control"
                    type="text"
                    name="category"
                    value={form.category}
                    onChange={onChange}
                    maxLength={100}
                    required/>
            </div>
            <div className="mb-3">
                <label className="form-label">Image</label>
                <input
                    type="file"
                    accept="image/*"
                    className="form-control"
                    onChange={onImageChange} />
                {imageFile ? <small>Selected: {imageFile.name}</small>:null}
            </div>
            {error ? <div className="alert alert-danger">{error}</div> : null}
            <button
                type="submit"
                className="btn btn-primary"
                disabled={loading}
            >{loading? "Saving...":submitText}</button>
        </form>
    )
}