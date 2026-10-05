import { Link } from 'react-router-dom'
import { useBakers } from '../hooks/useBakers'

function BakerList() {
    const { bakers, loading, error } = useBakers()

    if (loading) return <p>Loading bakers...</p>
    if (error) return <p>Error: {error}</p>

    return (
        <div className="d-flex flex-column align-items-center gap-3">
            <h2>Select Your Name</h2>
            {bakers.map(baker => (
                <Link key={baker.id} className="w-25 btn btn-lg btn-primary" to={`/bakers/${baker.id}`}>
                    {baker.name}
                </Link>
            ))}
        </div>
    )
}

export default BakerList