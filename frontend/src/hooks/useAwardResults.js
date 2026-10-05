import { useState, useEffect } from 'react'

export function useAwardResults() {
    const [awardResults, setAwardResults] = useState([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState('')

    useEffect(() => {
        fetch('http://localhost:8080/api/award-results')
            .then(res => {
                if (!res.ok) return res.text().then(msg => { throw new Error(msg) })
                return res.json()
            })
            .then(data => setAwardResults(data))
            .catch(err => setError(err.message))
            .finally(() => setLoading(false))
    }, [])

    return { awardResults, loading, error }
}