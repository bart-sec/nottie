import { useEffect, useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from './assets/vite.svg'
import heroImg from './assets/hero.png'
import './App.css'







function App() {
  const [count, setCount] = useState(0)


  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    fetch("http://localhost:5241/weatherforecast")
      .then((response) => {
        if (!response.ok) {
          throw new Error('Error response not ok');
        }

        return response.json();
      })
      .then(data => {
        console.log("setting data");
        console.log(data);
        setData(data);
        setLoading(false);
      })
      .catch(error => {
        console.log("setting error");
        setError(error);
        setLoading(false);
      })
  }, [])


  return (
    <>
      <section id="center">
        <div className="hero">
          <img src={heroImg} className="base" width="170" height="179" alt="" />
          <img src={reactLogo} className="framework" alt="React logo" />
          <img src={viteLogo} className="vite" alt="Vite logo" />
        </div>
        <div>
          <h1>Get started</h1>
          <p>
            Edit <code>src/App.tsx</code> and save to test <code>HMR</code>
          </p>
        </div>
        <button
          type="button"
          className="counter"
          onClick={() => setCount((count) => count + 1)}
        >
          Count is {count}
        </button>


        {error && <div>{error.data}</div>}

        {loading ? "is loading" :

          <ul>
            {data.map((entry, index) => <li key={index}><p>date: {entry.date}</p><p>temperature: {entry.temperatureC}</p></li>)}
          </ul>}

      </section>


      <section id="spacer"></section>
    </>
  )
}

export default App
