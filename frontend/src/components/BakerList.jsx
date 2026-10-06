import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { loginBaker, logoutBaker } from '../api'
import { useBakers } from '../hooks/useBakers'
import { useCurrentBaker } from '../hooks/useCurrentBaker'

function BakerList() {
    const navigate = useNavigate()
    const { bakers, loading, error } = useBakers()
    const { baker: currentBaker, setBaker: setCurrentBaker, loading: sessionLoading } = useCurrentBaker()
    const [selectedBaker, setSelectedBaker] = useState(null)
    const [pin, setPin] = useState('')
    const [pinError, setPinError] = useState('')
    const [submitting, setSubmitting] = useState(false)

    if (loading || sessionLoading) return <p>Loading bakers...</p>
    if (error) return <p>Error: {error}</p>

    async function handlePinSubmit(event) {
        event.preventDefault()
        if (!selectedBaker || !pin.trim()) {
            setPinError('Enter a PIN to continue.')
            return
        }

        setSubmitting(true)
        setPinError('')
        try {
            await loginBaker(selectedBaker.bakerName, pin.trim())
            setCurrentBaker({ id: selectedBaker.id, bakerName: selectedBaker.bakerName })
            navigate(`/bakers/${selectedBaker.id}`)
        } catch (err) {
            setPinError(err.message)
        } finally {
            setSubmitting(false)
        }
    }

    async function handleLogout() {
        try {
            await logoutBaker()
            setCurrentBaker(null)
        } catch (err) {
            setPinError(err.message)
        }
    }

    return (
        <div className="d-flex flex-column align-items-center gap-3">
            <h2>Who are you?</h2>
            {currentBaker && (
                <div className="alert alert-info">
                    Signed in as {currentBaker.bakerName}.
                    <button type="button" className="btn btn-link" onClick={handleLogout}>Sign out</button>
                </div>
            )}
            {bakers.map(baker => (
                <button
                    key={baker.id}
                    type="button"
                    className="w-25 btn btn-lg btn-primary"
                    onClick={() => {
                        setSelectedBaker(baker)
                        setPin('')
                        setPinError('')
                    }}
                >
                    {baker.bakerName}{baker.hasPin ? '' : ' (set PIN)'}
                </button>
            ))}

            {selectedBaker && (
                <form className="card card-body w-50" onSubmit={handlePinSubmit}>
                    <h3>{selectedBaker.hasPin ? `Enter PIN for ${selectedBaker.bakerName}` : `Create a PIN for ${selectedBaker.bakerName}`}</h3>
                    <p>{selectedBaker.hasPin ? 'Enter your PIN to sign in.' : 'No PIN is registered. The PIN you create will be saved for future sign-ins.'}</p>
                    <label htmlFor="baker-pin" className="form-label">PIN</label>
                    <input
                        id="baker-pin"
                        className="form-control"
                        type="password"
                        autoComplete="current-password"
                        value={pin}
                        onChange={event => setPin(event.target.value)}
                        autoFocus
                    />
                    {pinError && <p className="text-danger mt-2">{pinError}</p>}
                    <div className="d-flex gap-2 mt-3">
                        <button className="btn btn-primary" type="submit" disabled={submitting}>
                            {submitting ? 'Please wait...' : selectedBaker.hasPin ? 'Sign in' : 'Create PIN and sign in'}
                        </button>
                        <button className="btn btn-secondary" type="button" onClick={() => setSelectedBaker(null)}>Cancel</button>
                    </div>
                </form>
            )}
        </div>
    )
}

export default BakerList