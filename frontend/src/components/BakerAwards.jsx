import { Link, useParams } from 'react-router-dom'
import { useBakers } from '../hooks/useBakers'
import { useCurrentBaker } from '../hooks/useCurrentBaker'
import Awards from './Awards'

function BakerAwards() {
    const { bakerId } = useParams()
    const { bakers, loading, error } = useBakers()
    const { baker: currentBaker, loading: sessionLoading, error: sessionError } = useCurrentBaker()

    if (loading || sessionLoading) return <p>Loading...</p>
    if (error) return <p>Error: {error}</p>
    if (sessionError) return <p>Error: {sessionError}</p>

    const baker = bakers.find(b => String(b.id) === bakerId)

    if (!baker) {
        return <p>Baker not found.</p>
    }

    if (!currentBaker || String(currentBaker.id) !== bakerId) {
        return <p>Please <Link to="/bakers">sign in as {baker.bakerName}</Link> before submitting awards.</p>
    }

    return <Awards excludeBakerId={baker.id} />
}

export default BakerAwards