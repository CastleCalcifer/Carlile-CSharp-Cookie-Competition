import { useEffect, useState } from 'react'
import { getCurrentBaker } from '../api'

export function useCurrentBaker() {
    const [baker, setBaker] = useState(null)
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState('')

    useEffect(() => {
        getCurrentBaker()
            .then(setBaker)
            .catch(err => setError(err.message))
            .finally(() => setLoading(false))
    }, [])

    return { baker, setBaker, loading, error }
}
