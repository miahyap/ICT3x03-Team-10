import { initialState, render, screen, mockResponse, fireEvent } from './utils';
import fetchMock from 'fetch-mock';
import Register from '../pages/Register';
import configureMockStore from 'redux-mock-store';
import thunk from 'redux-thunk';
import { verifyOauth, checkUsername, checkPassword, signUp, generateOTP } from '../actions/userActions';

const middlewares = [thunk];
const mockStore = configureMockStore(middlewares);

describe('Register UI', () => {

    beforeEach(() => {
        render(<Register />,{ initialState: initialState });
    });

    test('Renders register page', () => {
        expect(screen.getByText('Register')).toBeInTheDocument();
    });

    test('Ensures username and password textfields are found', () => {
        const gettingStartedText = "To get started creating an account, please validate your SIT Student Account.";
        expect(screen.getByText(gettingStartedText)).toBeInTheDocument();
    });

    test('Ensure fullname and email fields are disabled', () => {
        const fullnameInput = screen.getByTestId('fullName').querySelector('input');
        const emailInput = screen.getByTestId('email').querySelector('input');
        expect(fullnameInput).toBeDisabled();
        expect(emailInput).toBeDisabled();
    });

});

describe('Register Redux actions', () => {

    afterEach(() => {
        fetchMock.reset();
        fetchMock.restore();
    });

    test('Verify Oauth Request', async () => {

        const oauthResponse = {
            "name": "TEST TEST",
            "email": "1800123@sit.singaporetech.edu.sg",
            "registerCode": "b4aeb641-170f-4a10-8ff9-8b0018ed9d03",
        };

        global.fetch = jest.fn().mockImplementation(() =>
        Promise.resolve(mockResponse(200, JSON.stringify(oauthResponse))));
        const data = {
            token: '0.AAAAfx-ZZNZEjE2c1Hhi6MuUxkbnETv03KlAomnv1GG6dEM- AEg.'
        };
        const store = mockStore(initialState);
        await store.dispatch(verifyOauth(data));
        const actions = store.getActions();
        expect(actions[0].type).toEqual("USER_REGISTER_OAUTH_SUCCESS");
    });


    test('Check username response', async () => {

        const checkUsernameResponse = {
            "status": true,
            "message": "Username is valid"
        };

        global.fetch = jest.fn().mockImplementation(() =>
        Promise.resolve(mockResponse(200, JSON.stringify(checkUsernameResponse))));
        const data = {
            token: 'b4aeb641-170f-4a10-8ff9-8b0018ed9d03',
            data: 'goodusername123'
        };
        const store = mockStore(initialState);
        await store.dispatch(checkUsername(data));
        const actions = store.getActions();
        expect(actions[0].type).toEqual('USER_REGISTER_USERNAME_REQUEST');
        expect(actions[1].type).toEqual('USER_REGISTER_USERNAME_VALID');
    });


    test('Check password response', async () => {
        const checkPasswordResponse = {
            "status": true,
            "message": "Password is valid"
        };

        global.fetch = jest.fn().mockImplementation(() =>
        Promise.resolve(mockResponse(200, JSON.stringify(checkPasswordResponse))));
        const data = {
            token: 'b4aeb641-170f-4a10-8ff9-8b0018ed9d03',
            data: 'goodpassword123'
        };
        const store = mockStore(initialState);
        await store.dispatch(checkPassword(data));
        const actions = store.getActions();
        expect(actions[0].type).toEqual('USER_REGISTER_PASSWORD_REQUEST');
        expect(actions[1].type).toEqual('USER_REGISTER_PASSWORD_VALID');
    });


    test('Generate OTP Response', async () => {
        const generateOtpResponse = {
            status: "Ok",
            message: "otpauth://totp/forumx?secret=LW3RDYPRPW233BE6WBRHIV4MBGCDWBRWQCLGNTK34F6A64YT"
        };

        global.fetch = jest.fn().mockImplementation(() =>
        Promise.resolve(mockResponse(200, JSON.stringify(generateOtpResponse))));
        const data = {
            token: "b4aeb641-170f-4a20-8ff9-8b0017ed9d03"
        };
        const store = mockStore(initialState);
        await store.dispatch(generateOTP(data));
        const actions = store.getActions();
        expect(actions[0].type).toEqual('USER_REGISTER_OTP_SUCCESS');
    });


    test('Check register response', async () => {

        global.fetch = jest.fn().mockImplementation(() =>
        Promise.resolve(mockResponse(200, null)));
        const data = {
            token: "b4aeb641-170f-4a20-8ff9-8b0017ed9d03",
            username: "goodusername123",
            password: "goodpassword123"
        };
        const store = mockStore(initialState);
        await store.dispatch(signUp(data));
        const actions = store.getActions();
        expect(actions[0].type).toEqual('USER_REGISTER_SUCCESS');
    });

});

describe('Register state changes', () => {

    test('Necessary fields are displayed', () => {
        const newState = {
            ...initialState,
            register: {
                isOauthSuccess: true,
                errorMessage: ''
            }
        }
        render(<Register />,{ initialState: newState });
        const usernameTextField = screen.getByTestId('username');
        const passwordTextField = screen.getByTestId('password');
        const confirmPasswordTextField = screen.getByTestId('confirmPassword');
        const generateOtpButton = screen.getByTestId('generateOtpButton');
        expect(usernameTextField).toBeInTheDocument();
        expect(passwordTextField).toBeInTheDocument();
        expect(confirmPasswordTextField).toBeInTheDocument();
        expect(generateOtpButton).toBeDisabled();
    });

    test('CheckUsername button to be enabled if username is filled', () => {
        const newState = {
            ...initialState,
            register: {
                isOauthSuccess: true,
                errorMessage: ''
            }
        }
        render(<Register />,{ initialState: newState });
        const usernameInputField = screen.getByTestId('username').querySelector('input');
        fireEvent.change(usernameInputField, { target: { value: 'test123' }});
        const checkUsernameButton = screen.getByTestId('checkUsernameButton');
        expect(checkUsernameButton).not.toBeDisabled();
    });

    test('Checkpassword button to be enabled if password and confirm password filled', () => {
        const newState = {
            ...initialState,
            register: {
                isOauthSuccess: true,
                errorMessage: ''
            }
        };
        render(<Register />,{ initialState: newState });
        const passwordInputField = screen.getByTestId('password').querySelector('input');
        const confirmPasswordTextField = screen.getByTestId('confirmPassword').querySelector('input');

        fireEvent.change(passwordInputField, { target: { value: 'test1234!' }});
        fireEvent.change(confirmPasswordTextField, { target: { value: 'test1234!' }});
        const checkPasswordButton = screen.getByTestId('checkPasswordButton');
        expect(checkPasswordButton).not.toBeDisabled();
    });

    test('Generate OTP Button to be clickable', () => {
        const newState = {
            ...initialState,
            register: {
                isOauthSuccess: true,
                errorMessage: '',
                usernameValid: true,
                passwordValid: true,
            }
        }
        render(<Register />,{ initialState: newState });
        expect(screen.getByTestId('generateOtpButton')).not.toBeDisabled();
    });

    test('Register button clickable and displayed', () => {
        const newState = {
            ...initialState,
            register: {
                isOauthSuccess: true,
                errorMessage: '',
                usernameValid: true,
                passwordValid: true,
                otpGenerated: true,
            }
        }
        render(<Register />,{ initialState: newState });
        expect(screen.getByTestId('registerButton')).toBeInTheDocument();
        expect(screen.getByTestId('registerButton')).not.toBeDisabled();
    });

});
