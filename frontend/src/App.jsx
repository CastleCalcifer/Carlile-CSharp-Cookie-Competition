import { Routes, Route } from 'react-router-dom'
import Nav from './components/Nav'
import Home from './components/Home'
import Voting from './components/Voting'
import BakerList from "./components/BakerList";
import BakerVoting from "./components/BakerVoting";
import Awards from './components/Awards'
import BakerAwards from "./components/BakerAwards";
import Results from './components/Results'
import './App.css'


function App() {
    return (
        <div className="app">
            <Nav />
            <main>
                <Routes>
                    <Route path="/" element={<Home />} />
                    <Route path="/voting" element={<Voting nextPath="/awards" />} />
                    <Route path="/bakers" element={<BakerList />} />
                    <Route path="/bakers/:bakerId" element={<BakerVoting />} />
                    <Route path="/awards" element={<Awards />} />
                    <Route path="/bakers/:bakerId/awards" element={<BakerAwards />} />
                    <Route path="/results" element={<Results />} />
                </Routes>
            </main>
        </div>
    )
}

export default App