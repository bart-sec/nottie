import { useKeycloak } from "@react-keycloak/web";

import Login from "./Login";


type SecurityContent = {
  children: React.ReactNode;
}

export default function SecurityGate({ children }: SecurityContent) {

  const { keycloak } = useKeycloak();

  const isLoggedIn = keycloak.authenticated;

  return isLoggedIn ? children : <Login />;
}
