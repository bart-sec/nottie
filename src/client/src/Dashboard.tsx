import { useKeycloak } from "@react-keycloak/web";
import { useEffect, useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from './assets/vite.svg'
import heroImg from './assets/hero.png'
import './App.css'


export default function Dashboard() {

  const { keycloak } = useKeycloak();

  const token = keycloak.token;

  const [count, setCount] = useState(0)


  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [reload, setReload] = useState(false);

  const deleteTodo = async (id: number) => {
    const response = await fetch("http://localhost:5241/todos/" + id, {
      method: "DELETE",
      headers: {
        "Authorization": `Bearer ${token}`
      }
    });
    if (response.ok) {
      console.log("Todo removed");

      setReload(true);
    }
  }


  const update = async (id: number, isCompleted: boolean) => {
    const response = await fetch("http://localhost:5241/todos/" + id,
      {
        method: "PATCH",
        headers: {
          "Content-Type": "application/json",
          "Authorization": `Bearer ${token}`
        },
        body: JSON.stringify({
          "isCompleted": isCompleted
        })
      });
    if (response.ok) {
      console.log("todo updated!");
      setReload(true);
    }
  }
  const submit = async () => {

    const response = await fetch("http://localhost:5241/todos", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`
      },
      body: JSON.stringify({
        "name": "new todo item",
        "isCompleted": false
      }),
    });
    if (response.ok) {
      console.log("Todo created!");
      setReload(true);
    }

  }

  useEffect(() => {
    fetch("http://localhost:5241/todos", {
      headers: {
        "Authorization": `Bearer ${token}`
      }
    })
      .then((response) => {
        if (!response.ok) {
          throw new Error('Error response not ok');
        }

        setReload(false);

        return response.json();
      })
      .then(data => {
        console.log("setting data");
        console.log(data);
        setData(data);
        setLoading(false);
        setReload(false);
      })
      .catch(error => {
        console.log("setting error");
        setError(error);
        setLoading(false);
        setReload(false);
      })
  }, [reload])



  return (
    <>
      <div className="secure-page">
        <p>welcome, {keycloak.tokenParsed?.sub}</p>
        <p>token: {keycloak.token}</p>
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
          <button type='button' onClick={submit}>button</button>

          {error && <div>{error.data}</div>}

          {loading ? "is loading" :

            <ul>
              {data.map(entry =>
                <li key={entry.id}>
                  <p>id: {entry.id}</p>
                  <p>name: {entry.name}</p>
                  <p>is completed: <input type='checkbox' value={entry.isCompleted} onChange={() => update(entry.id, !entry.isCompleted)} /> {entry.isCompleted}</p>
                  <p><button type='button' onClick={() => deleteTodo(entry.id)}>delete</button></p>
                </li>)}
            </ul>}

        </section>


        <section id="spacer"></section>

      </div>
    </>
  )
}

