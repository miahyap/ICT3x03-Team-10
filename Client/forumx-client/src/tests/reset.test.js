import { initialState, render, screen, mockResponse } from './utils';
import fetchMock from 'fetch-mock';
import ResetPassword from '../pages/ResetPassword';
import configureMockStore from 'redux-mock-store';
import thunk from 'redux-thunk';
import { resetPassword } from '../actions/userActions';

const middlewares = [thunk];
const mockStore = configureMockStore(middlewares);

describe('Reset Password UI', () => {

    beforeEach(() => {
        render(<ResetPassword />,{ initialState: initialState });
    });

    test('Renders reset password page', () => {
        expect(screen.getByTestId('validateAccountButton')).toBeInTheDocument();
        expect(screen.getByTestId('validateAccountButton')).not.toBeDisabled();
        expect(screen.getByTestId('username')).toBeInTheDocument();
        expect(screen.getByTestId('password')).toBeInTheDocument();
        expect(screen.getByTestId('resetPasswordButton')).toBeDisabled();
    });

    test('Checks username and password helper text', () => {
        const usernameInputField = screen.getByTestId('username').querySelector('#username-helper-text');
        const passwordInputField = screen.getByTestId('password').querySelector('#password-helper-text');
        expect(usernameInputField.textContent).toBe('Please enter username');
        expect(passwordInputField.textContent).toBe('Please enter password');
    });

});

describe('Reset Password Redux actions', () => {

    afterEach(() => {
        fetchMock.reset();
        fetchMock.restore();
    });

    test('Reset Password request', async () => {

        global.fetch = jest.fn().mockImplementation(() =>
        Promise.resolve(mockResponse(200, null)));
        const data = {
            Token: '0.AAAAfx-ZZNZEjE2c1Hhi6MuUxkbnETv03KlAomnv1GG6dEM',
            Username: 'test12345',
            Password: '123456'
        };
        const store = mockStore(initialState);
        await store.dispatch(resetPassword(data));
        const actions = store.getActions();
        expect(actions[0].type).toEqual('USER_RESET_PASSWORD_SUCCESS');
    });
});