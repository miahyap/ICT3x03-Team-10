import React from 'react';
// import './__tests__/node_modules/@testing-library/jest-dom/extend-expect';
import App from './App';
import { initialState, render, screen } from './tests/utils';


it('Renders logs in when application starts', () => {
  render(<App />,{ initialState: initialState })

  expect(screen.getByText("Sign in")).toBeInTheDocument();
});
