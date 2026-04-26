import axios from 'axios'

const api = axios.create({

    // baseURL: 'http://Productsite242-env.eba-gxfpcrry.eu-north-1.elasticbeanstalk.com/api',
    baseURL: 'http://localhost:5093/api',
})

export default api