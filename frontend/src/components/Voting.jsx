import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useCookies } from '../hooks/useCookies'
import { useCurrentBaker } from '../hooks/useCurrentBaker'
import { submitVotes } from '../api'

function Voting({ excludeBakerId, nextPath }) {
    const navigate = useNavigate()
    const { baker: currentBaker, loading: sessionLoading, error: sessionError } = useCurrentBaker()
    const { cookies, loading, error: loadError } = useCookies({ excludeBakerId: excludeBakerId ?? currentBaker?.id })

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

        const allRanked = cookies.length > 0 && cookies.every(cookie => rankings[cookie.id])
        if (!allRanked) {
            setError('Please rank all cookies before submitting.')
            return
        }
        setError('')

        const cookieIds = Object.entries(rankings)
            .sort(([, rankA], [, rankB]) => Number(rankA) - Number(rankB))
            .map(([cookieId]) => Number(cookieId))

        submitVotes(cookieIds)
            .then(() => navigate(nextPath))
            .catch(err => setError(err.message))
    }

    if (sessionLoading || loading) return <p>Loading cookies...</p>
    if (sessionError) return <p>Error: {sessionError}</p>
    if (loadError) return <p>Error: {loadError}</p>

    return (
        <div style={{ backgroundColor: '#d7e5f0' }}>
            <h2>Voting</h2>
            {cookies.length === 0 && <p>No cookies are available for voting.</p>}
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
                    <button type="submit" className="btn btn-primary submitButton" disabled={cookies.length === 0}>Submit</button>
                </div>
            </form>
        </div>
    )
}

export default Voting