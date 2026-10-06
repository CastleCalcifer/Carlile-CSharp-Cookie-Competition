import { useState, useEffect } from 'react'
import { getBakers } from '../api'

export function useBakers() {
    const [bakers, setBakers] = useState([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState('')

    useEffect(() => {
        getBakers()
            .then(data => setBakers(data))
            .catch(err => setError(err.message))
            .finally(() => setLoading(false))
    }, [])

    return { bakers, loading, error }
}