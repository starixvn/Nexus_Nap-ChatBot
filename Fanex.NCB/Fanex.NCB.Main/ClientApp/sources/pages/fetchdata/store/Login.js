const receiveLoginDataType = 'RECEIVE_LOGIN_DATA';
const initialState = { token: 'abc', isAuthenticated: false};

export const actionCreators = {
  login: params => async (dispatch, getState) => {      

    const url = `api/Users/Authenticate`;
    const response = await fetch(url, {
      method: 'POST',
      headers: {
        'Accept': 'application/json, text/plain, */*',
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(params)
    });
    const data = await response.json();
    
    dispatch({ type: receiveLoginDataType, data });
  }
};

export const reducer = (state, action) => {
  state = state || initialState;

  if (action.type === receiveLoginDataType) {
    return {
      ...state,
      token: action.data.token,
      isAuthenticated: true
    };
  }

  return state;
};
