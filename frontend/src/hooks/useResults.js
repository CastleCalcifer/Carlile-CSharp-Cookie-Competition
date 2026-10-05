import { useState, useEffect } from 'react'

export function useResults() {
    const [results, setResults] = useState([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState('')

    useEffect(() => {
        fetch('http://localhost:8080/api/results')
            .then(res => {
                if (!res.ok) throw new Error('Failed to load results')
                return res.json()
            })
            .then(data => setResults(data))
            .catch(err => setError(err.message))
            .finally(() => setLoading(false))
    }, [])

    return { results, loading, error }
}