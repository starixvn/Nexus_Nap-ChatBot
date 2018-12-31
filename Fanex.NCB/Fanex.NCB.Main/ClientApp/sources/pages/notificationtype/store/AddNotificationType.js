const freshAddNotificationType = "FRESH_ADD_NOTIFICATION_TYPE";
const addNotificationType = "ADD_NOTIFICATION_TYPE";
const editNotificationType = "EDIT_NOTIFICATION_TYPE";

export const actionCreators = {
    addNewNotificationType : (params) => async(dispatch, getState) => {
        const url = '/NotificationType/Add';
        const response = await fetch(url, {
            method: 'POST',
            body: JSON.stringify(params)
        });
        const data = await response.json();     

        dispatch({type: addNotificationType, data});
    },

    editNewNotificationType : (params) => async(dispatch, getState) => {
        const url = '/NotificationType/Edit';
        const response = await fetch(url, {
            method: 'POST',
            body: JSON.stringify(params)
        });
        const data = await response.json();     

        dispatch({type: editNotificationType, data});
    },

    refreshAddNotificationType : () => async(dispatch, getState) => {       
        dispatch({type: freshAddNotificationType});
    }
};

export const reducer = (state, action) => {
    state = state || {addedresult: false};

    if(action.type === freshAddNotificationType){
        console.log("freshAddNotificationType");
        return  {
            ...state,
            addedresult: false
        };;
    }

    if(action.type === addNotificationType){        
        return {
            ...state,
            addedresult: action.data
        };
    }

    if(action.type === editNotificationType){        
        return {
            ...state,
            addedresult: action.data
        };
    }

    
    return state;
}