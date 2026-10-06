import { useResults } from '../hooks/useResults'

function Results() {
    const { results, loading, error } = useResults()

    if (loading) return <p>Loading results...</p>
    if (error) return <p>Error: {error}</p>

    const ranked = results.ranked || []
    const awards = [
        { label: 'Most Creative', winner: results.awards?.creative, points: 'creativePoints' },
        { label: 'Best Presentation', winner: results.awards?.presentation, points: 'presentationPoints' },
    ]

    return (
        <>
            <h2>Results</h2>
            <div className="row">
                {ranked.map((result, index) => (
                    <div
                        key={result.id}
                        className="d-flex flex-column justify-content-center align-items-center cookieOption"
                    >
                        <div className="p-2">
                            <h2>{placeLabel(index)}</h2>
                            <h2>{result.cookieName}</h2>
                            <img src={result.imageUrl} className="cookieImage" alt={result.cookieName} />
                            <h2>Total Score: {result.score}</h2>
                        </div>
                    </div>
                ))}
            </div>

            <h2>Awards</h2>
            {awards.map(({ label, winner, points }) => winner && (
                <div
                    key={label}
                    className="d-flex flex-column justify-content-center align-items-center cookieOption"
                >
                    <div className="p-2">
                        <h2>{label}</h2>
                        <h2>{winner.cookieName}</h2>
                        <img src={winner.imageUrl} className="cookieImage" alt={winner.cookieName} />
                        <h2>Points: {winner[points]}</h2>
                    </div>
                </div>
            ))}
        </>
    )
}

function placeLabel(index) {
    const place = index + 1
    const suffix = { 1: 'st', 2: 'nd', 3: 'rd' }[place] || 'th'
    return `${place}${suffix} Place`
}

export default Results