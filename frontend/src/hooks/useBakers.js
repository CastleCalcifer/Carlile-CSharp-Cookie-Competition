import { useState, useEffect } from 'react'

export function useBakers() {
    const [bakers, setBakers] = useState([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState('')

    useEffect(() => {
        fetch('http://localhost:8080/api/bakers')
            .then(res => {
                if (!res.ok) throw new Error('Failed to load bakers')
                return res.json()
            })
            .then(data => setBakers(data))
            .catch(err => setError(err.message))
            .finally(() => setLoading(false))
    }, [])

    return { bakers, loading, error }
}