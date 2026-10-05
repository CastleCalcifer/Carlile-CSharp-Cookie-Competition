import { Link } from 'react-router-dom'

const menuItems = [
    { label: 'Voting', icon: '/images/vote.png', to: '/voting' },
    { label: 'Baker Voting', icon: '/images/bakers.png', to: '/bakers' },
    { label: 'Results', icon: '/images/results.png', to: '/results' },
]

function Home() {
    return (
        <>
            <h1>Welcome to the Annual <br /> Cookie Competition</h1>
            <div className="tagLine">
                <h2>Where we take sibling rivalry too far</h2>
            </div>
            <div className="container d-flex mx-auto justify-content-center gap-5">
                <div className="d-grid gap-5 col-5 mx-auto">
                    {menuItems.map(item => (
                        <div key={item.to} className="menuItem">
                            <Link
                                className="material-icons md-120 buttonIcon w-100 btn btn-lg p-5"
                                to={item.to}
                                role="button"
                            >
                                <img className="buttonImage" src={item.icon} alt={item.label} />
                            </Link>
                            <Link className="w-100 btn btn-lg p-5" to={item.to} role="button">
                                {item.label}
                            </Link>
                        </div>
                    ))}
                </div>
            </div>
        </>
    )
}

export default Home