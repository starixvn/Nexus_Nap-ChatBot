import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Modal, ModalHeader, ModalBody, Form, FormGroup, Label } from 'reactstrap';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import PropTypes from 'prop-types';
import { InputField } from '../../common/InputField';
import { SelectField } from '../../common/SelectField';
import { MultipleSelectField } from '../../common/MultipleSelectField';
import ConfirmPopup from '../../common/ConfirmPopup';
import { Notification } from '../../common/Notification';

import '@9softstudio/react-multiselect/dist/style.css';

export class UpdatePopup extends Component {
  constructor(props) {
    super(props);
    this.state = {
      notificationTypeId: this.props.updatePopupData.editingItem.notificationTypeId,
      notificationTypeName: {
        value: this.props.updatePopupData.editingItem.notificationTypeName.value,
        isValid: this.props.updatePopupData.editingItem.notificationTypeName.isValid
      },
      recipientTypeIds: {
        value: this.props.updatePopupData.editingItem.recipientTypeIds.value,
        isValid: this.props.updatePopupData.editingItem.recipientTypeIds.isValid
      },
      chatGroupTypeIds: {
        value: this.props.updatePopupData.editingItem.chatGroupTypeIds.value,
        isValid: this.props.updatePopupData.editingItem.chatGroupTypeIds.isValid
      },
      systemId: {
        value: this.props.updatePopupData.editingItem.systemId.value,
        isValid: this.props.updatePopupData.editingItem.systemId.isValid
      },
      isSummited: false,
      isShowedConfirm: false
    };
  }

  componentWillMount() {
    this.props.getSettings();
  }

  submitHandler = e => {
    e.preventDefault();

    !this.state.isSummited && this.setState({ isSummited: true });
    if (this.checkValidParams()) {
      const params = this.getParams();
      const isNew = this.props.updatePopupData.isNew;
      this.props.update(params, isNew).then(this.closePopup);
    }
  }

  getParams = () => {
    return {
      notificationTypeId: this.state.notificationTypeId,
      notificationTypeName: this.state.notificationTypeName.value,
      recipientTypeIds: this.state.recipientTypeIds.value,
      chatGroupTypeIds: this.state.chatGroupTypeIds.value,
      systemId: this.state.systemId.value
    };
  }

  checkValidParams = () => this.state.notificationTypeName.isValid && this.state.recipientTypeIds.isValid && this.state.chatGroupTypeIds.isValid && this.state.systemId.isValid;

  onChange = (fieldName, fieldValue) => {
    this.setState({
      [fieldName]: fieldValue
    });
  }

  closePopup = () => {
    setTimeout(function () {
        this.props.updateData.addedresult.isSuccessful && this.props.closeUpdatePopupHandler();
      }.bind(this),
    2000);
  }

  closeConfirmation = () => {
    this.setState({
      isShowedConfirm: false
    });
  }

  togglePopup = () => {
    this.setState({
      isShowedConfirm: true
    });
  }

  updateDropdownData = (items, selectedIds) => {
    let selectedIdsArr = selectedIds.split(',');
    items && items.map(v => {
      v['checked'] = selectedIdsArr.includes(v.id.toString());
    });
  }

  render() {
    this.updateDropdownData(this.props.settingData.recipientTypes, this.props.updatePopupData.editingItem.recipientTypeIds.value);
    this.updateDropdownData(this.props.settingData.chatGroupTypes, this.props.updatePopupData.editingItem.chatGroupTypeIds.value);

    return (
      <Modal isOpen toggle={this.togglePopup} wrapClassName="new-notification-type">
        <ModalHeader toggle={this.togglePopup}>{this.props.updatePopupData.popupTitle}</ModalHeader>
        <ModalBody>
          {this.props.updateData.addedresult &&
            <Notification
              isSuccess={this.props.updateData.addedresult.isSuccessful}
              successMsg={this.props.updatePopupData.successMsg}
              failedMsg={this.props.updateData.addedresult.message} />}

          <Form className="form-create">
            <FormGroup>
              <Label className="form-label required">Notification Type Name</Label>
              <InputField initialValue={this.props.updatePopupData.editingItem.notificationTypeName.value}
                isSummited={this.state.isSummited}
                onChange={value => this.onChange('notificationTypeName', value)}
              />
            </FormGroup>
            <FormGroup>
              <Label className="form-label required">Recipient Type</Label>
              <MultipleSelectField
                selectedValue={this.state.recipientTypeIds.value}
                data={this.props.settingData.recipientTypes}
                keyField={'id'}
                valueField={'name'}
                statusField={'checked'}
                isSummited={this.state.isSummited}
                onChange={value => this.onChange('recipientTypeIds', value)}
              />
            </FormGroup>
            <FormGroup>
              <Label className="form-label required">Chat Group Type</Label>
              <MultipleSelectField
                selectedValue={this.state.chatGroupTypeIds.value}
                data={this.props.settingData.chatGroupTypes}
                keyField={'id'}
                valueField={'name'}
                statusField={'checked'}
                isSummited={this.state.isSummited}
                onChange={value => this.onChange('chatGroupTypeIds', value)}
              />
            </FormGroup>
            <FormGroup>
              <Label className="form-label required">System</Label>
              <SelectField initialValue={this.props.updatePopupData.editingItem.systemId.value}
                data={this.props.settingData.notificationSystems}
                isSummited={this.state.isSummited}
                onChange={value => this.onChange('systemId', value)}
              />
            </FormGroup>
            <button className="btn-submit" onClick={this.submitHandler}>Submit</button>
          </Form>
        </ModalBody>

        <ConfirmPopup
          onConfirm={this.props.closeUpdatePopupHandler}
          onToggle={this.closeConfirmation}
          isOpen={this.state.isShowedConfirm}
          content={"There might be some unsaved changes in the form. The changes shall not be saved if you close this form. Are you sure you want to close this form?"}
        />
      </Modal>
    );
  }
}

UpdatePopup.propTypes = {
  closeUpdatePopupHandler: PropTypes.func.isRequired
};

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
  const { settingData, updatePopupData, updateData } = state;
  return {
    settingData,
    updatePopupData,
    updateData
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(UpdatePopup);