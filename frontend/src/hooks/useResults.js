import { useState, useEffect } from 'react'
import { getResults } from '../api'

export function useResults() {
    const [results, setResults] = useState({ ranked: [], awards: {} })
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState('')

    useEffect(() => {
        getResults()
            .then(data => setResults(data))
            .catch(err => setError(err.message))
            .finally(() => setLoading(false))
    }, [])

    return { results, loading, error }
}