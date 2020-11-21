import { history } from '../helpers/history';
import { refreshToken } from '../actions/userActions';


export function jwt({ dispatch, getState }) {

    return (next) => async (action) => {
        if (typeof action === 'function') {
            if (localStorage.getItem('access-token')) {
                let tokenExpiration = getState().user.expiry;
                const tokenExpirationConverted = Date.parse(tokenExpiration);
                // console.log(tokenExpirationConverted - Date.now() < 1800 );
                if (tokenExpiration && (tokenExpirationConverted - Date.now() < 1800 )){
                    //Ensure that token is not refreshed
                    if (Date.now() >= tokenExpirationConverted) {
                        localStorage.clear();
                        history.push('/login');
                        return next(action);
                    } else if (!getState().user.isTokenRefreshed){
                        await refreshToken(dispatch);
                        return next(action);
                    } else {
                        return next(action);
                    }
                }
            }
        }
        return next(action);
    };
}