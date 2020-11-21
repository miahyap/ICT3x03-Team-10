import { initialState, render, screen, fireEvent, mockResponse } from './utils';
import Login from '../pages/Login';
import fetchMock from 'fetch-mock';
import configureMockStore from 'redux-mock-store';
import thunk from 'redux-thunk';
import { login, verifyOTP } from '../actions/userActions';

const middlewares = [thunk];
const mockStore = configureMockStore(middlewares);

describe('Login UI', () => {

    beforeEach(() => {
        render(<Login />,{ initialState: initialState });
    });

    test('Renders logs in when application starts', () => {
        expect(screen.getByText("Sign in")).toBeInTheDocument();
    });

    test('Ensures username and password textfields are found', () => {
        expect(screen.getByTestId('username')).toBeInTheDocument();
        expect(screen.getByTestId('password')).toBeInTheDocument();
    });

    test('Ensure that Sign In button is disabled', () => {
        expect(screen.getByTestId('signInButton')).toHaveAttribute('disabled');
    });

    test('Check error is displayed if invalid username', () => {
        const input = screen.getByTestId('username').querySelector('input');
        fireEvent.change(input, { target: { value: '@@@' }});
        expect(screen.getByTestId('username').querySelector('#username-helper-text').textContent).toBe('Invalid Username!');
    });

    test('Login button clickable, if fields are correct', () => {
        const usernameInput = screen.getByTestId('username').querySelector('input');
        const passwordInput = screen.getByTestId('password').querySelector('input');
        fireEvent.change(usernameInput, { target: { value: 'testing123' }});
        fireEvent.change(passwordInput, { target: {value: '12345' }});
        expect(screen.getByTestId('signInButton')).not.toBeDisabled();
    });

    test('Login button unclickable, if fields are incorrect', () => {
        const usernameInput = screen.getByTestId('username').querySelector('input');
        const passwordInput = screen.getByTestId('password').querySelector('input');
        fireEvent.change(usernameInput, { target: { value: '@@@' }});
        fireEvent.change(passwordInput, { target: {value: '12345' }});
        expect(screen.getByTestId('signInButton')).toBeDisabled();
    });
});

describe('Login Redux actions', () => {

    afterEach(() => {
        fetchMock.reset();
        fetchMock.restore();
    });

    test('Login Request', async () => {

        const loginResponse = {"message": "218b4de7-fdfe-48c6-84bf-da3c2ec92539"};

        global.fetch = jest.fn().mockImplementation(() =>
        Promise.resolve(mockResponse(200, JSON.stringify(loginResponse))));
        const data = {
            Username: 'test12345',
            Password: '123456'
        };
        const store = mockStore(initialState);
        await store.dispatch(login(data));
        const actions = store.getActions();
        expect(actions[0].type).toEqual("USER_LOGIN_SUBMITTED_SUCCESS");

    });

    test('Verify OTP Request', async () => {

        const verifyOtpResponse = {
            "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI1OGNl",
            "username": "test123",
            "expiry": "2020-11-01T14:33:03Z"
        };
        global.fetch = jest.fn().mockImplementation(() =>
        Promise.resolve(mockResponse(200, JSON.stringify(verifyOtpResponse))));
        const data = {
            token: "218b4de7-fdfe-48c6-84bf-da3c2ec92539",
            "data": "958995"
        };
        const store = mockStore(initialState);
        await store.dispatch(verifyOTP(data));
        const actions = store.getActions();
        expect(actions[0].type).toEqual('USER_LOGIN_SUCCESS');
        expect(actions[0].data.token).toEqual(verifyOtpResponse.token);
        expect(actions[0].data.username).toEqual(verifyOtpResponse.username);
        expect(actions[0].data.expiry).toEqual(verifyOtpResponse.expiry);
    });

});

describe('Login state change', () => {

    beforeEach(() => {
        const newState = {
            ...initialState,
            user: {
                submitted: true,
                errorMessage: '',
            }
        }
        render(<Login />,{ initialState: newState });
    });

    test('OTP Fields can be seen', () => {
        const otpTextField = screen.getByTestId("otp");
        expect(otpTextField).toBeInTheDocument();
        fireEvent.change(otpTextField.querySelector('input'), { target: { value: 123 }});
        expect(otpTextField.querySelector('#otpDigit-helper-text').textContent).toBe('Invalid OTP');
    });

    test('Validate OTP button to be clickable', () => {
        const validateOtpButton = screen.getByTestId("validateOtpButton");
        expect(validateOtpButton).toBeInTheDocument();
        expect(validateOtpButton).not.toBeDisabled();
    });
});
