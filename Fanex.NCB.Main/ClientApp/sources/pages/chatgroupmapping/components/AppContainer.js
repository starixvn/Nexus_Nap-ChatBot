import React, { Component } from 'react';
import { connect } from 'react-redux';
import Filter from './Filter';
import DataGrid from './DataGrid';
import { NCBBreadcrum } from '../../common/breadcrumbs';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import UpdatePopup from './UpdatePopup';
import "../../../fonts/style.css";
import './AppContainer.scss';

class AppContainer extends Component {
    constructor(props) {
        super(props);

        this.state = {
            data: [{
                id: 100,
                name: 100
            }
            ]
        }
    }

    togglePopup() {

    }   

    closeUpdatePopupHandler = () => {
        this.props.closeEditPopup();
        this.props.refreshUpdateData();
        //this.props.updateActiveRowIndex(-1);
        !this.props.updatePopupData.isShowed && this.props.getGridData(this.props.gridParams);
    }

    render() {        
        return (
            <div>
                <NCBBreadcrum chatbotName="Skype Chatbot" pageTitle="Chat Group Mapping"></NCBBreadcrum>
                <Filter />
                <DataGrid />
                {this.props.updatePopupData.isShowed && <UpdatePopup closeUpdatePopupHandler={this.closeUpdatePopupHandler} />}
            </div>
        );
    }
}

function mapDispatchToProps(dispatch) {
    return bindActionCreators(ActionCreators, dispatch);
}

function mapStateToProps(state) {
    const { gridParams, updatePopupData } = state;
    return Object.assign({}, { gridParams, updatePopupData });
}

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);