import { useParams, Link } from 'react-router-dom'
import { useBakers } from '../hooks/useBakers'
import { useCurrentBaker } from '../hooks/useCurrentBaker'
import Voting from './Voting'

function BakerVoting() {
    const { bakerId } = useParams()
    const { bakers, loading: bakersLoading, error: bakersError } = useBakers()
    const { baker: currentBaker, loading: sessionLoading, error: sessionError } = useCurrentBaker()
    if (bakersLoading || sessionLoading) return <p>Loading...</p>
    if (bakersError) return <p>Error: {bakersError}</p>
    if (sessionError) return <p>Error: {sessionError}</p>

    const baker = bakers.find(b => String(b.id) === bakerId)

    if (!baker) {
        return <p>Baker not found.</p>
    }

    if (!currentBaker || String(currentBaker.id) !== bakerId) {
        return <p>Please <Link to="/bakers">sign in as {baker.bakerName}</Link> before voting.</p>
    }

    return <Voting excludeBakerId={baker.id} nextPath={`/bakers/${bakerId}/awards`} />
}

export default BakerVoting