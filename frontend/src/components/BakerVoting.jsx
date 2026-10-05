import { useParams } from 'react-router-dom'
import { useBakers } from '../hooks/useBakers'
import { useCookies } from '../hooks/useCookies'
import Voting from './Voting'

function BakerVoting() {
    const { bakerId } = useParams()
    const { bakers, loading: bakersLoading, error: bakersError } = useBakers()
    const { cookies, loading: cookiesLoading, error: cookiesError } = useCookies()

    if (bakersLoading || cookiesLoading) return <p>Loading...</p>
    if (bakersError) return <p>Error: {bakersError}</p>
    if (cookiesError) return <p>Error: {cookiesError}</p>

    const baker = bakers.find(b => String(b.id) === bakerId)

    if (!baker) {
        return <p>Baker not found.</p>
    }

    const ownCookie = cookies.find(c => c.baker?.id === baker.id)

    return <Voting excludeCookieId={ownCookie?.id} nextPath={`/bakers/${bakerId}/awards`} />
}

export default BakerVoting