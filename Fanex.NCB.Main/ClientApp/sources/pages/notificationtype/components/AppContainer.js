import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import { NCBBreadcrum } from '../../common/breadcrumbs/breadcrumbs';
import DataGrid from './DataGrid';
import UpdatePopup from './UpdatePopup';
import DeletePopup from './DeletePopup';

import '../../shared/components/font.css';
import './AppContainer.scss';

export class AppContainer extends Component {
  constructor(props) {
    super(props);
  }

  componentWillMount() {
    this.props.getIsHost();
  }

  closeUpdatePopupHandler = () => {
    this.props.closeNewEditPopup();
    this.props.refreshUpdateData();
    this.props.updateActiveRowIndex(-1);
    !this.props.updatePopupData.isShowed && this.props.getGridData(this.props.gridParams);
  }

  closeDeletePopupHandler = () => {
    this.props.closeDeletePopup();
    this.props.refreshDeleteData();
    this.props.updateActiveRowIndex(-1);
    !this.props.deletePopupData.isShowed && this.props.getGridData(this.props.gridParams);
  }

  render() {
    return (
      <div>
        <NCBBreadcrum chatbotName="Skype Chatbot" pageTitle="Notification Type" />
        {this.props.isHost && <button className="btn-new" onClick={this.props.openNewPopup}>New Notification Type</button>}
        <DataGrid />
        {this.props.updatePopupData.isShowed && <UpdatePopup closeUpdatePopupHandler={this.closeUpdatePopupHandler} />}
        {this.props.deletePopupData.isShowed && <DeletePopup closeDeletePopupHandler={this.closeDeletePopupHandler} />}
      </div>
    );
  }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
  const { gridParams, updatePopupData, deletePopupData, isHost } = state;
  return {
    isHost,
    gridParams,
    updatePopupData,
    deletePopupData
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);