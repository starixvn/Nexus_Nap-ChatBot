import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import { Button, Modal, ModalHeader, ModalBody, Form, FormGroup, Label, Input, FormFeedback } from 'reactstrap';
import { Formik } from 'formik';
import { object, string, boolean } from 'yup';
import Labels from '../../common/Labels';
import ResponseMessage from '../../common/response-message/ResponseMessage';

export class EditMapping extends Component {
    constructor(props) {
        super(props);

        this.state = {};
    }

    componentDidMount = () => {
        this.props.setEditMappingResult(null);
        this.props.getMsgContentsTypes();
        const { editMappingPopup } = this.props;

        if (editMappingPopup.mapping) {
            this.props.getNotificationTypeRef(editMappingPopup.mapping.notificationTypeID);
            const formik = this.refs.form;

            formik.setValues({
                ...formik.state.values,
                isTest: editMappingPopup.mapping.isTest,
                notificationTypeId: editMappingPopup.mapping.notificationTypeID,
                notificationTemplateId: editMappingPopup.mapping.notificationTemplateID,
                requiredParam: editMappingPopup.mapping.requiredParams,
                msgContentsType: editMappingPopup.mapping.msgContentsType,
                oldMsgContentsType: editMappingPopup.mapping.msgContentsType,
                oldNotificationTypeId: editMappingPopup.mapping.notificationTypeID,
                oldNotificationTemplateId: editMappingPopup.mapping.notificationTemplateID
            });
        }
    }

    schema = object().shape({
        jobId: string(),
        notificationTypeId: string().required(Labels.FieldRequired),
        notificationTemplateId: string().required(Labels.FieldRequired),
        msgContentsType: string().required(Labels.FieldRequired),
        requiredParam: string().required(Labels.FieldRequired),
        isTest: boolean(),
        oldMsgContentsType: string(),
        oldNotificationTypeId: string(),
        oldNotificationTemplateId: string()
    })

    handleNotificationTypeChange = event => {
        const formik = this.refs.form;
        this.props.getNotificationTypeRef(event.target.value);

        formik.setValues({
            ...formik.state.values,
            notificationTypeId: event.target.value
        });
    }

    handleTemplateChange = event => {
        const formik = this.refs.form;

        formik.setValues({
            ...formik.state.values,
            notificationTemplateId: event.target.value
        });
    }

    handleMsgContentsTypeChange = event => {
        const formik = this.refs.form;

        formik.setValues({
            ...formik.state.values,
            msgContentsType: event.target.value
        });
    }

    handleRequiredParamChange = event => {
        const formik = this.refs.form;

        formik.setValues({
            ...formik.state.values,
            requiredParam: event.target.value
        });
    }

    handleChangeIsTest = event => {
        const formik = this.refs.form;

        formik.setValues({
            ...formik.state.values,
            isTest: !formik.state.values.isTest
        });
    }

    renderNotificationTypeOption = item => <option key={item.notificationTypeID} value={item.notificationTypeID} >{item.notificationTypeName}</option>

    renderNotificationTemplateOption = item => <option key={item.notificationTemplateId} value={item.notificationTemplateId} >{item.notificationTemplateName}</option>

    renderMsgContentsTypeOption = item => <option key={item.type} value={item.type} >{item.name}</option>

    renderForm = ({
        values,
        errors,
        touched,
        handleChange,
        handleSubmit,
        setFieldValue,
        isSubmitting
    }) => {
        return (
            <Form onSubmit={handleSubmit}>
                <input name='jobId' className='hidden' defaultValue={this.props.jobInfo && this.props.jobInfo.id} />
                <FormGroup>
                    <Label for="notificationTypeId" className="form-label required">Notification Type</Label>
                    <div>
                        <Input type="select"
                            invalid={touched.notificationTypeId && errors.notificationTypeId}
                            name="notificationTypeId"
                            onChange={this.handleNotificationTypeChange}
                            value={values.notificationTypeId}
                        >
                            {this.props.notificationTypes && this.props.notificationTypes.map(this.renderNotificationTypeOption)}
                        </Input>
                        {touched.notificationTypeId && errors.notificationTypeId && <FormFeedback>{errors.notificationTypeId}</FormFeedback>}
                    </div>
                </FormGroup>
                <FormGroup>
                    <Label for="notificationTemplateId" className="form-label required">Notification Template</Label>
                    <div>
                        <Input type="select"
                            invalid={touched.notificationTemplateId && errors.notificationTemplateId}
                            name="notificationTemplateId"
                            onChange={this.handleTemplateChange}
                            value={values.notificationTemplateId}
                        >
                            {this.props.notificationTypeRef && this.props.notificationTypeRef.templates && this.props.notificationTypeRef.templates.map(this.renderNotificationTemplateOption)}
                        </Input>
                        {touched.notificationTemplateId && errors.notificationTemplateId && <FormFeedback>{errors.notificationTemplateId}</FormFeedback>}
                    </div>
                </FormGroup>
                <FormGroup>
                    <Label className="form-label">System</Label>
                    <Label className="form-label">
                        {this.props.notificationTypeRef && this.props.notificationTypeRef.systemName ? this.props.notificationTypeRef.systemName : ''}
                    </Label>
                </FormGroup>
                <FormGroup>
                    <Label className="form-label">Recipient Type</Label>
                    <Label className="form-label">
                        {this.props.notificationTypeRef && this.props.notificationTypeRef.recipientTypeNames ? this.props.notificationTypeRef.recipientTypeNames : ''}
                    </Label>
                </FormGroup>
                <FormGroup>
                    <Label for="msgContentsType" className="form-label required">Msg Contents Type</Label>
                    <div>
                        <Input type="select"
                            invalid={touched.msgContentsType && errors.msgContentsType}
                            name="msgContentsType"
                            onChange={this.handleMsgContentsTypeChange}
                            value={values.msgContentsType}
                        >
                            {this.props.msgContentsTypes && this.props.msgContentsTypes.map(this.renderMsgContentsTypeOption)}
                        </Input>
                        {touched.msgContentsType && errors.msgContentsType && <FormFeedback>{errors.msgContentsType}</FormFeedback>}
                    </div>
                </FormGroup>
                <FormGroup>
                    <Label for="requiredParam" className="form-label required">Required Param</Label>
                    <div>
                        <Input type='textarea'
                            rows={4}
                            invalid={touched.requiredParam && errors.requiredParam}
                            name="requiredParam"
                            onChange={this.handleRequiredParamChange}
                            value={values.requiredParam}
                            maxLength={300}
                        />
                        {touched.requiredParam && errors.s && <FormFeedback>{errors.requiredParam}</FormFeedback>}
                    </div>
                </FormGroup>
                <FormGroup>
                    <Label for="isTest" className="form-label">Is Test</Label>
                    <div>
                        <input id='is-test'
                            className="checkbox-custom"
                            type='checkbox'
                            checked={values.isTest}
                            value={values.isTest}
                            onChange={this.handleChangeIsTest}
                        />
                        <label htmlFor='is-test' />
                        {touched.isTest && errors.isTest && <FormFeedback>{errors.isTest}</FormFeedback>}
                    </div>
                </FormGroup>
                <div className="list-button">
                    <Button className="btn-submit float-right" disabled={isSubmitting} type="submit">{Labels.Save}</Button>
                </div>
            </Form >
        );
    }

    handleSubmit = (values, { setSubmitting }) => {
        setSubmitting(true);
        this.props.editMapping(values).then(() => {
            if (this.props.editMappingResult.isSuccessful) {
                setTimeout(() => {
                    this.props.onEditSuccess();
                }, 1000);
            } else {
                setSubmitting(false);
            }
        });
    }

    render() {
        let settingType = this.props.settingTypes.filter(x => this.props.jobInfo && x.id === this.props.jobInfo.id);
        let title = settingType && settingType.length > 0 ? `${Labels.EditMappingSetting} for ${settingType[0].name}` : Labels.EditMappingSetting;
        return (
            <Modal isOpen toggle={this.props.onClose} backdrop="static">
                <ModalHeader toggle={this.props.onClose}>{title}</ModalHeader>
                <ModalBody>
                    {this.props.editMappingResult &&
                        <ResponseMessage isSuccessful={this.props.editMappingResult.isSuccessful}
                            message={this.props.editMappingResult.message || Labels.UpdateMappingSettingSuccessfully}
                        />
                    }
                    <Formik ref="form"
                        initialValues={{
                            jobId: this.props.jobInfo && this.props.jobInfo.id,
                            isTest: false
                        }}
                        render={this.renderForm}
                        validationSchema={this.schema}
                        onSubmit={this.handleSubmit}
                    />
                </ModalBody>
            </Modal>
        );
    }
}

const mapDispatchToProps = dispatch => {
    return bindActionCreators({
        editMapping: ActionCreators.editMapping,
        getMsgContentsTypes: ActionCreators.getMsgContentsTypes,
        getNotificationTypeRef: ActionCreators.getNotificationTypeRef,
        setNotificationTypeRef: ActionCreators.setNotificationTypeRef,
        setEditMappingResult: ActionCreators.setEditMappingResult
    }, dispatch);
};

const mapStateToProps = state => {
    const {
        editMappingPopup,
        editMappingResult,
        notificationTypes,
        msgContentsTypes,
        notificationTypeRef,
        jobInfo,
        settingTypes
    } = state;

    return {
        editMappingPopup,
        editMappingResult,
        notificationTypes,
        msgContentsTypes,
        notificationTypeRef,
        jobInfo,
        settingTypes
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(EditMapping);