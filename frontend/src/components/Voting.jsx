import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useCookies } from '../hooks/useCookies'

function Voting({ excludeCookieId, nextPath }) {
    const navigate = useNavigate()
    const { cookies: allCookies, loading, error: loadError } = useCookies()

    const cookies = excludeCookieId
        ? allCookies.filter(cookie => cookie.id !== excludeCookieId)
        : allCookies

    const rankOptions = cookies.map((i, index) => String(index + 1))

    const [rankings, setRankings] = useState({})
    const [error, setError] = useState('')


    function handleRankChange(cookieId, value) {
        setRankings(prev => ({ ...prev, [cookieId]: value }))
    }

    function availableRanksFor(cookieId) {
        const takenByOthers = Object.entries(rankings)
            .filter(([id, rank]) => id !== String(cookieId) && rank !== '')
            .map(([, rank]) => rank)

        return rankOptions.filter(rank => !takenByOthers.includes(rank))
    }

    function handleSubmit(e) {
        e.preventDefault()

        const allRanked = Object.values(rankings).every(rank => rank !== '')
        if (!allRanked) {
            setError('Please rank all cookies before submitting.')
            return
        }
        setError('')

        const payload = Object.fromEntries(
            Object.entries(rankings).map(([cookieId, rank]) => [cookieId, Number(rank)])
        )

        fetch('http://localhost:8080/api/votes', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload),
        })
            .then(res => {
                if (!res.ok) return res.text().then(msg => { throw new Error(msg) })
                navigate(nextPath)
            })
            .catch(err => setError(err.message))
    }

    if (loading) return <p>Loading cookies...</p>
    if (loadError) return <p>Error: {loadError}</p>

    return (
        <div style={{ backgroundColor: '#d7e5f0' }}>
            <h2>Voting</h2>
            <form onSubmit={handleSubmit} className="row">
                {cookies.map(cookie => (
                    <div key={cookie.id} className="d-flex flex-column justify-content-center align-items-center cookieOption">
                        <div className="p-2">
                            <img src={cookie.image} className="cookieImage" alt={cookie.cookieName} />
                        </div>
                        <div className="d-inline-flex p-2 gap-4">
                            <p>{cookie.cookieName}</p>
                            <div className="rankDropdown">
                                <select
                                    className="form-select"
                                    value={rankings[cookie.id] || ''}
                                    onChange={(e) => handleRankChange(cookie.id, e.target.value)}
                                >
                                    <option value="" disabled>Select rank</option>
                                    {availableRanksFor(cookie.id).map(rank => (
                                        <option key={rank} value={rank}>{rank}</option>
                                    ))}
                                </select>
                            </div>
                        </div>
                    </div>
                ))}

                {error && <p className="text-danger">{error}</p>}

                <div className="d-flex flex-column justify-content-center align-items-center cookieOption">
                    <button type="submit" className="btn btn-primary submitButton">Submit</button>
                </div>
            </form>
        </div>
    )
}

export default Voting