import { useResults } from '../hooks/useResults'
import { useAwardResults } from '../hooks/useAwardResults'

function Results() {
    const { results, loading: resultsLoading, error: resultsError } = useResults()
    const { awardResults, loading: awardsLoading, error: awardsError } = useAwardResults()

    if (resultsLoading || awardsLoading) return <p>Loading results...</p>
    if (resultsError) return <p>Error: {resultsError}</p>

    const ranked = [...results].reverse()

    const awardLabels = {
        most_creative: 'Most Creative',
        best_presentation: 'Best Presentation',
    }

    return (
        <>
            <h2>Results</h2>
            <div className="row">
                {ranked.map((result, index) => (
                    <div
                        key={result.cookie.id}
                        className="d-flex flex-column justify-content-center align-items-center cookieOption"
                    >
                        <div className="p-2">
                            <h2>{placeLabel(index, ranked.length)}</h2>
                            <h2>{result.cookie.cookieName}</h2>
                            <img src={result.cookie.image} className="cookieImage" alt={result.cookie.cookieName} />
                            <h2>Total Score: {result.totalScore}</h2>
                        </div>
                    </div>
                ))}
            </div>

            <h2>Awards</h2>
            {awardsError && <p>{awardsError}</p>}
            {!awardsError && awardResults.map(award => (
                <div
                    key={award.awardType}
                    className="d-flex flex-column justify-content-center align-items-center cookieOption"
                >
                    <div className="p-2">
                        <h2>{awardLabels[award.awardType] || award.awardType}</h2>
                        <h2>{award.cookie.cookieName}</h2>
                        <img src={award.cookie.image} className="cookieImage" alt={award.cookie.cookieName} />
                        <h2>Votes: {award.voteCount}</h2>
                    </div>
                </div>
            ))}
        </>
    )
}

function placeLabel(index, total) {
    const placeFromLast = total - index
    const suffix = { 1: 'st', 2: 'nd', 3: 'rd' }[placeFromLast] || 'th'
    return `${placeFromLast}${suffix} Place`
}

export default Results