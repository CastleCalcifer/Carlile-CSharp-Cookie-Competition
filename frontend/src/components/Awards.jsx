import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useCookies } from '../hooks/useCookies'

const placeholderImage = 'https://www.jocooks.com/wp-content/uploads/2021/12/sugar-cookies-1-17.jpg'

const awards = [
    { id: 'most_creative', label: 'Most Creative' },
    { id: 'best_presentation', label: 'Best Presentation' },
]

function Awards({ excludeCookieId }) {
    const navigate = useNavigate()
    const { cookies: allCookies, loading, error: loadError } = useCookies()

    const cookies = excludeCookieId
        ? allCookies.filter(cookie => cookie.id !== excludeCookieId)
        : allCookies

    const [selections, setSelections] = useState({
        most_creative: '',
        best_presentation: '',
    })
    const [error, setError] = useState('')

    function handleSelectionChange(awardId, cookieId) {
        setSelections(prev => ({ ...prev, [awardId]: cookieId }))
    }

    function imageFor(awardId) {
        const selectedId = selections[awardId]
        const selectedCookie = cookies.find(c => String(c.id) === selectedId)
        return selectedCookie ? selectedCookie.image : placeholderImage
    }

    function handleSubmit(e) {
        e.preventDefault()

        const allSelected = Object.values(selections).every(cookieId => cookieId !== '')
        if (!allSelected) {
            setError('Please select a cookie for every award.')
            return
        }
        setError('')

        const payload = Object.fromEntries(
            Object.entries(selections).map(([awardId, cookieId]) => [awardId, Number(cookieId)])
        )

        fetch('http://localhost:8080/api/awards', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload),
        })
            .then(res => {
                if (!res.ok) return res.text().then(msg => { throw new Error(msg) })
                navigate('/results')
            })
            .catch(err => setError(err.message))
    }

    if (loading) return <p>Loading cookies...</p>
    if (loadError) return <p>Error: {loadError}</p>

    return (
        <div className="row">
            <form onSubmit={handleSubmit}>
                {awards.map(award => (
                    <div key={award.id} className="d-flex flex-column justify-content-center align-items-center cookieOption">
                        <div className="p-2">
                            <h2>{award.label}</h2>
                            <img src={imageFor(award.id)} alt={award.label} />
                        </div>
                        <div className="d-inline-flex p-2">
                            <div className="awardDropdown">
                                <select
                                    className="form-select"
                                    value={selections[award.id]}
                                    onChange={(e) => handleSelectionChange(award.id, e.target.value)}
                                >
                                    <option value="" disabled>Select cookie</option>
                                    {cookies.map(cookie => (
                                        <option key={cookie.id} value={cookie.id}>{cookie.cookieName}</option>
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

export default Awards