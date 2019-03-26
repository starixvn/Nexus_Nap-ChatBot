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
import Labels from '../../common/Labels';

export class AppContainer extends Component {
    constructor(props) {
        super(props);       
    }
   

    closeUpdatePopupHandler = () => {
        this.props.closeEditPopup();
        this.props.refreshUpdateData();
        !this.props.updatePopupData.isShowed && this.props.getGridData(this.props.gridParams);
    }

    render() {        
        return (
            <div>
                <NCBBreadcrum chatbotName={Labels.SkypeChatbot} pageTitle={Labels.ChatGroupMapping} />
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