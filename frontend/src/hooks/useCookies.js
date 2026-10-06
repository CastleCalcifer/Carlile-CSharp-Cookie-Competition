import { useState, useEffect } from 'react'
import { getCookies } from '../api'

export function useCookies({ excludeBakerId } = {}) {
    const [cookies, setCookies] = useState([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState('')

    useEffect(() => {
        getCookies({ excludeBakerId })
            .then(data => setCookies(data.map(cookie => {
                const imageUrl = cookie.imageUrl || cookie.image
                const image = imageUrl && !/^(?:[a-z]+:)?\/\//i.test(imageUrl) && !imageUrl.startsWith('/')
                    ? `/${imageUrl}`
                    : imageUrl
                return { ...cookie, image }
            })))
            .catch(err => setError(err.message))
            .finally(() => setLoading(false))
    }, [excludeBakerId])

    return { cookies, loading, error }
}