import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import { Button, Modal, ModalHeader, ModalBody, Form, FormGroup, Label, Input, FormFeedback, Col } from 'reactstrap';
import { Formik } from 'formik';
import { object, string, boolean } from 'yup';
import Labels from '../../common/Labels';
import ResponseMessage from '../../common/response-message/ResponseMessage';

class NewMapping extends Component {
    constructor(props) {
        super(props);
    }

    componentDidMount = () => {
        this.props.setCreateMappingResult(null);
        this.props.getMsgContentsTypes();
        this.props.setTemplates(null);
        this.props.setNotificationTypeRef(null)
        const formik = this.refs.form;

        formik.setValues({
            ...formik.state.values,
            isTest: true
        });
    }

    schema = object().shape({
        jobId: string(),
        notificationTypeId: string().required(Labels.FieldRequired),
        notificationTemplateId: string().required(Labels.FieldRequired),
        msgContentsType: string().required(Labels.FieldRequired),
        requiredParam: string().required(Labels.FieldRequired),
        isTest: boolean()
    })

    handleNotificationTypeChange = event => {
        const formik = this.refs.form;
        this.props.getTemplates(event.target.value);
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
                <FormGroup className='vertical-row'>
                    <Label for="notificationTypeId" className="form-label required">Notification Type</Label>
                    <div className='input-row-container'>
                        <Input type="select"
                            invalid={touched.notificationTypeId && errors.notificationTypeId}
                            name="notificationTypeId"
                            onChange={this.handleNotificationTypeChange}
                            value={values.notificationTypeId}
                        >
                            <option value="" disabled hidden>{Labels.PleaseSelect}</option>
                            {this.props.notificationTypes && this.props.notificationTypes.map(this.renderNotificationTypeOption)}
                        </Input>
                        {touched.notificationTypeId && errors.notificationTypeId && <FormFeedback>{errors.notificationTypeId}</FormFeedback>}
                    </div>
                </FormGroup>
                <FormGroup className='vertical-row'>
                    <Label for="notificationTemplateId" className="form-label required">Notification Template</Label>
                    <div className='input-row-container'>
                        <Input type="select"
                            invalid={touched.notificationTemplateId && errors.notificationTemplateId}
                            name="notificationTemplateId"
                            onChange={this.handleTemplateChange}
                            value={values.notificationTemplateId}
                        >
                            <option value="" disabled hidden>{Labels.PleaseSelect}</option>
                            {this.props.notificationTemplates && this.props.notificationTemplates.map(this.renderNotificationTemplateOption)}
                        </Input>
                        {touched.notificationTemplateId && errors.notificationTemplateId && <FormFeedback>{errors.notificationTemplateId}</FormFeedback>}
                    </div>
                </FormGroup>
                <FormGroup className='vertical-row'>
                    <Label for="systemName" className="form-label">System</Label>
                    <div className='input-row-container'>
                        <Input className='disabled-input'
                            name="systemName"
                            disabled={true}
                            value={this.props.notificationTypeRef ? this.props.notificationTypeRef.systemName : ''}
                        />
                    </div>
                </FormGroup>
                <FormGroup className='vertical-row'>
                    <Label for="recipientTypeId" className="form-label">Recipient Type</Label>
                    <div className='input-row-container'>
                        <Input className='disabled-input'
                            name="recipientTypeId"
                            disabled={true}
                            value={this.props.notificationTypeRef ? this.props.notificationTypeRef.recipientTypeName : ''}
                        />
                    </div>
                </FormGroup>
                <FormGroup className='vertical-row'>
                    <Label for="msgContentsType" className="form-label required">Msg Contents Type</Label>
                    <div className='input-row-container'>
                        <Input type="select"
                            invalid={touched.msgContentsType && errors.msgContentsType}
                            name="msgContentsType"
                            onChange={this.handleMsgContentsTypeChange}
                            value={values.msgContentsType}
                        >
                            <option value="" disabled hidden>{Labels.PleaseSelect}</option>
                            {this.props.msgContentsTypes && this.props.msgContentsTypes.map(this.renderMsgContentsTypeOption)}
                        </Input>
                        {touched.msgContentsType && errors.msgContentsType && <FormFeedback>{errors.msgContentsType}</FormFeedback>}
                    </div>
                </FormGroup>
                <FormGroup className='vertical-row'>
                    <Label for="requiredParam" className="form-label required">Required Param</Label>
                    <div className='input-row-container'>
                        <Input invalid={touched.requiredParam && errors.requiredParam}
                            name="requiredParam"
                            onChange={this.handleRequiredParamChange}
                            value={values.requiredParam}
                        />
                        {touched.requiredParam && errors.requiredParam && <FormFeedback>{errors.requiredParam}</FormFeedback>}
                    </div>
                </FormGroup>
                <FormGroup className='vertical-row'>
                    <Label for="isTest" className="form-label">Is Test</Label>
                    <div className='input-row-container'>
                        <input id='is-test'
                            className="checkbox-custom"
                            type='checkbox'
                            defaultChecked={true}
                            defaultValue={values.isTest}
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
        this.props.createMapping(values).then(() => {
            if (this.props.createMappingResult.isSuccessful) {
                setTimeout(() => {
                    this.props.onCreateSuccess();
                }, 1000);
            } else {
                setSubmitting(false);
            }
        });
    }

    render() {
        return (
            <Modal isOpen toggle={this.props.onClose} className="new-setting-type" backdrop="static">
                <ModalHeader toggle={this.props.onClose}>Create New Mapping Setting</ModalHeader>
                <ModalBody>
                    {this.props.createMappingResult &&
                        <ResponseMessage isSuccessful={this.props.createMappingResult.isSuccessful}
                            message={this.props.createMappingResult.message || Labels.CreateNewMappingSettingSuccessfully}
                        />
                    }
                    <Formik ref="form"
                        initialValues={{
                            jobId: this.props.jobInfo && this.props.jobInfo.id,
                            notificationTypeId: '',
                            notificationTemplateId: '',
                            msgContentsType: '',
                            requiredParam: '',
                            isTest: true
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
        createMapping: ActionCreators.createMapping,
        getTemplates: ActionCreators.getTemplates,
        getMsgContentsTypes: ActionCreators.getMsgContentsTypes,
        getNotificationTypeRef: ActionCreators.getNotificationTypeRef,
        setTemplates: ActionCreators.setTemplates,
        setNotificationTypeRef: ActionCreators.setNotificationTypeRef,
        setCreateMappingResult: ActionCreators.setCreateMappingResult
    }, dispatch);
};

const mapStateToProps = state => {
    const {
        createMappingResult,
        notificationTypes,
        notificationTemplates,
        msgContentsTypes,
        notificationTypeRef,
        jobInfo
    } = state;

    return {
        createMappingResult,
        notificationTypes,
        notificationTemplates,
        msgContentsTypes,
        notificationTypeRef,
        jobInfo
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(NewMapping);