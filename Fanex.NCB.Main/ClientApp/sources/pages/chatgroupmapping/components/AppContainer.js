import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import Filter from './Filter';
import DataGrid from './DataGrid';
import { NCBBreadcrum } from '../../common/breadcrumbs/breadcrumbs';
import { ActionCreators } from '../actions';
import EditPopup from './EditPopup';
import './AppContainer.scss';
import Labels from '../../common/Labels';

export class AppContainer extends Component {
    constructor(props) {
        super(props);
    }

    handleCloseEditPopup = () => {
        this.props.setEditPopupData({ isShowed: false, editingItem: {} });
        this.props.setEditResult(null);
        !this.props.editPopupData.isShowed && this.props.getGridData(this.props.gridParams);
    }

    render() {
        return (
            <div>
                <NCBBreadcrum chatbotName={Labels.SkypeChatbot} pageTitle={Labels.ChatGroupMapping} />
                <Filter />
                <DataGrid />
                {
                    this.props.editPopupData.isShowed &&
                    <EditPopup onClosePopup={this.handleCloseEditPopup} />}
            </div>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { gridParams, editPopupData } = state;
    return {
        gridParams,
        editPopupData
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);