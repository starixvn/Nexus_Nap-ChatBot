import React, { Component } from 'react';
import { connect } from 'react-redux';
import Filter from './Filter';
import DataGrid from './DataGrid';
import { NCBBreadcrum } from '../../common/breadcrumbs';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import UpdatePopup from './UpdatePopup';
import './AppContainer.scss';
import Labels from '../../common/Labels';

export class AppContainer extends Component {
    constructor(props) {
        super(props);
    }

    componentDidMount() {
        console.log(this.props);
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

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { gridParams, updatePopupData } = state;
    return {
        gridParams,
        updatePopupData
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);