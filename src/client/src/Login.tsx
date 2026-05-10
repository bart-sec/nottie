// import type { KeycloakAdapter } "@react-keycloak/keycloak-ts"

import { useKeycloak } from "@react-keycloak/web";

// class CustomAdapter implements KeycloakAdapter {

// };


export default function Login() {
  const { keycloak } = useKeycloak();

  return (
    <div>
      <button onClick={() => keycloak.login()}>Login</button>
    </div>
  )
}
