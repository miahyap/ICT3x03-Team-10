import { createContext, useContext } from 'react';

export const AuthContext = createContext({
    loggedIn: false,
});

export function useAuth() {
    return useContext(AuthContext);
}
