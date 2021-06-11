import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Modal, ModalHeader, ModalBody, Form, FormGroup, Label } from 'reactstrap';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions';
import { InputField } from '../../common/input-field/InputField';
import { SelectField } from '../../common/select-field/SelectField';
import { MultipleSelectField } from '../../common/multiple-select-field/MultipleSelectField';
import ConfirmPopup from '../../common/confirm-popup/ConfirmPopup';
import { toast } from 'react-toastify';

export class CreateEditPopup extends Component {
    constructor(props) {
        super(props);
        this.state = {
            notificationTypeId: this.props.createEditPopupData.editingItem.notificationTypeId,
            notificationTypeName: {
                value: this.props.createEditPopupData.editingItem.notificationTypeName.value,
                isValid: this.props.createEditPopupData.editingItem.notificationTypeName.isValid
            },
            recipientTypeIds: {
                value: this.props.createEditPopupData.editingItem.recipientTypeIds.value,
                isValid: this.props.createEditPopupData.editingItem.recipientTypeIds.isValid
            },
            chatGroupTypeIds: {
                value: this.props.createEditPopupData.editingItem.chatGroupTypeIds.value,
                isValid: this.props.createEditPopupData.editingItem.chatGroupTypeIds.isValid
            },
            systemId: {
                value: this.props.createEditPopupData.editingItem.systemId.value,
                isValid: this.props.createEditPopupData.editingItem.systemId.isValid
            },
            isSummited: false,
            isShowedConfirm: false,
            isChanged: false,
            isLoading: false
        };
    }

    componentWillMount() {
        this.props.getSettings();
    }

    submitHandler = e => {
        e.preventDefault();

        this.setState(
            {
                isSummited: true,
                isLoading: true
            },
            () => {
                if (this.checkValidParams()) {
                    const params = this.getParams();
                    const isNew = this.props.createEditPopupData.isNew;
                    this.props.createEditNotificationType(params, isNew).then(this.closePopup);
                } else {
                    this.setState({ isLoading: false });
                }
            }
        );
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

    checkValidParams = () => this.state.notificationTypeName.isValid
        && this.state.recipientTypeIds.isValid
        && this.state.chatGroupTypeIds.isValid
        && this.state.systemId.isValid;

    onChange = (fieldName, fieldValue) => {
        this.setState({
            isChanged: true,
            [fieldName]: fieldValue
        });
    }

    closePopup = () => {
        if (this.props.createEditResult) {
            if (this.props.createEditResult.isSuccessful) {
                toast.success(this.props.createEditPopupData.successMsg);
            } else {
                toast.error(this.props.createEditResult.message);
            }
        }

        if (this.props.createEditResult.isSuccessful) {
            this.props.onClose();
        } else {
            setTimeout(function () { this.setState({ isLoading: false }); }.bind(this), 4000);
        }
    }

    closeConfirmation = () => {
        this.setState({
            isShowedConfirm: false
        });
    }

    togglePopup = () => {
        if (this.state.isChanged) {
            this.setState({
                isShowedConfirm: this.state.isChanged
            });
        } else {
            this.props.onClose();
        }
    }

    updateDropdownData = (items, selectedIds) => {
        let selectedIdsArr = selectedIds.split(',');
        return items ? items.map(v => {
            return { ...v, checked: selectedIdsArr.includes(v.id.toString()) };
        }): [];
    }

    render() {
        const { settings, createEditPopupData } = this.props;
        const recipientTypes = this.updateDropdownData(settings.recipientTypes, this.state.recipientTypeIds.value);
        const chatGroupTypes = this.updateDropdownData(settings.chatGroupTypes, this.state.chatGroupTypeIds.value);

        return (
            <Modal isOpen wrapClassName="new-notification-type">
                <ModalHeader toggle={this.togglePopup}>{createEditPopupData.popupTitle}</ModalHeader>
                <ModalBody>
                    <Form className="form-create">
                        <FormGroup>
                            <Label className="form-label required">Notification Type Name</Label>
                            <InputField initialValue={this.state.notificationTypeName.value}
                                maxLength={200}
                                isSummited={this.state.isSummited}
                                onChange={value => this.onChange('notificationTypeName', value)}
                            />
                        </FormGroup>
                        <FormGroup>
                            <Label className="form-label required">Recipient Type</Label>
                            <MultipleSelectField
                                selectedValue={this.state.recipientTypeIds.value}
                                data={recipientTypes}
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
                                data={chatGroupTypes}
                                keyField={'id'}
                                valueField={'name'}
                                statusField={'checked'}
                                isSummited={this.state.isSummited}
                                onChange={value => this.onChange('chatGroupTypeIds', value)}
                            />
                        </FormGroup>
                        <FormGroup>
                            <Label className="form-label required">System</Label>
                            <SelectField initialValue={this.state.systemId.value}
                                data={settings.notificationSystems}
                                isSummited={this.state.isSummited}
                                onChange={value => this.onChange('systemId', value)}
                            />
                        </FormGroup>
                        <button disabled={this.state.isLoading} className="btn-submit" onClick={this.submitHandler}>Save</button>
                    </Form>
                </ModalBody>

                <ConfirmPopup
                    onConfirm={this.props.onClose}
                    onToggle={this.closeConfirmation}
                    isOpen={this.state.isShowedConfirm}
                    content={"There might be some unsaved changes in the form. The changes shall not be saved if you close this form. Are you sure you want to close this form?"}
                />
            </Modal>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { settings, createEditPopupData, createEditResult } = state;
    return {
        settings,
        createEditPopupData,
        createEditResult
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(CreateEditPopup);