import { Link } from 'react-router-dom'

function Nav() {
    return (
        <div className="header">
            <nav className="navbar navbar-expand-lg navbar-light fixed-top" style={{ backgroundColor: '#9dccf0' }}>
                <Link className="navbar-brand" to="/">Cookie Competition</Link>

                <button
                    className="navbar-toggler"
                    type="button"
                    data-bs-toggle="collapse"
                    data-bs-target="#navbarSupportedContent"
                    aria-controls="navbarSupportedContent"
                    aria-expanded="false"
                    aria-label="Toggle navigation"
                >
                    <span className="navbar-toggler-icon"></span>
                </button>

                <div className="collapse navbar-collapse" id="navbarSupportedContent">
                    <ul className="navbar-nav me-auto">
                        <li className="nav-item active">
                            <Link className="nav-link" to="/voting">Voting</Link>
                        </li>
                        <li className="nav-item">
                            <Link className="nav-link" to="/bakers">Baker sign-in</Link>
                        </li>
                        <li className="nav-item">
                            <Link className="nav-link" to="/results">Results</Link>
                        </li>
                        <li className="nav-item">
                            <Link className="nav-link" to="/awards">Awards</Link>
                        </li>
                    </ul>
                </div>
            </nav>
        </div>
    )
}

export default Nav