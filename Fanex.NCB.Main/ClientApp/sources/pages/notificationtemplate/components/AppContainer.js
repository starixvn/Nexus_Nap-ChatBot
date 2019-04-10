import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import { NCBBreadcrum } from '../../common/breadcrumbs';
import Filter from './Filter';
import DataGrid from './DataGrid';
import UpdatePopup from './UpdatePopup';
import DeletePopup from './DeletePopup';
import './AppContainer.scss';

export class AppContainer extends Component {
    constructor(props) {
        super(props);
    }

    closeUpdatePopupHandler = () => {
        //this.props.closeNewEditPopup();
        //this.props.refreshUpdateData();
        //!this.props.updatePopupData.isShowed && this.props.getGridData(this.props.gridParams);
    }

    closeDeletePopupHandler = () => {
        //this.props.closeDeletePopup();
       // this.props.updateActiveRowIndex(-1);
       // !this.props.deletePopupData.isShowed && this.props.getGridData(this.props.gridParams);
    }

    render() {
        return (
            <div>
                <NCBBreadcrum chatbotName="Skype Chatbot" pageTitle="Notification Template" />
                <Filter />
                <DataGrid />

                {this.props.updatePopupData.isShowed && <UpdatePopup closeUpdatePopupHandler={this.closeUpdatePopupHandler} />}
                {this.props.deletePopupData.isShowed && <DeletePopup closeDeletePopupHandler={this.closeDeletePopupHandler} />}
            </div>
        );
    }
}

function mapDispatchToProps(dispatch) {
    return bindActionCreators(ActionCreators, dispatch);
}

function mapStateToProps(state) {
    const { gridParams, updatePopupData, deletePopupData } = state;
    return Object.assign({}, { gridParams, updatePopupData, deletePopupData });
}

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);
