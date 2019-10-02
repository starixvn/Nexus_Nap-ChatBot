import './AppContainer.scss';
import "react-mde/lib/styles/css/react-mde-all.css";

import React from 'react';
import { connect } from 'react-redux';
import { DateTime } from 'luxon';
import { Form, FormGroup, Label, Button, Input, FormFeedback } from 'reactstrap';
import { stringify } from 'qs';
import { Formik } from 'formik';
import { object, string, array, date } from 'yup';
import ReactMde from 'react-mde';
import { NCBBreadcrum } from '../../../common/breadcrumbs/breadcrumbs';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../../actions/index';
import TreeSelect from '../../../common/TreeSelect/index';
import DatePicker from '../../../common/DatePicker/index';
import Labels from '../../../common/Labels';
import { DateTimeFormat, editorCommandList, invalidFieldClass } from '../../../constants/constants';
import ResponseMessage from '../../../common/response-message/ResponseMessage';
import Site from '../../../lib/site';
import ConfirmPopup from '../../../common/confirm-popup/ConfirmPopup';
import ContentPreview from '../../../common/ContentPreview';

const SendModeOptions = [
    {
        value: '1',
        label: 'Send Immediately'
    },
    {
        value: '2',
        label: 'Send At'
    }
];

const Today = new Date(new Date().toLocaleString("en-US", { timeZone: "Asia/Taipei" }));

export class AppContainer extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            isSummited: false,
            manuallyEditted: false,
            template: {
                title: {
                    originTemplate: '',
                    middleTemplate: '',
                    processedTemplate: '',
                    finalTemplate: ''
                },
                content: {
                    originTemplate: '',
                    middleTemplate: '',
                    processedTemplate: '',
                    finalTemplate: ''
                },
                params: []
            }
        };
    }

    componentDidMount() {
        this.props.getProducts();
        this.props.getNotificationTypes();
    }

    handleToggleConfirmationPopup = () => {
        this.props.toggleConfirmationPopup(!this.props.confimrationPopup.isOpen);
        this.refs.form.setSubmitting(false);
    }

    handleToggleResetConfirmationPopup = () => {
        this.props.toggleResetConfirmationPopup(false);
    }

    handleToggleEditConfirmationPopup = () => {
        this.props.toggleEditConfirmationPopup(false);
    }

    handleConfirm = () => {
        this.props.toggleConfirmationPopup(false);

        const formik = this.refs.form;
        const values = formik.state.values;

        const recipientIds = [];
        this.getRecipientIds(recipientIds, this.selectedNodes);

        const data = stringify({
            notificationTypeId: values.notificationTypeId,
            productId: values.productId,
            recipientIds: recipientIds.toString(),
            title: values.title,
            content: values.content,
            sendDate: DateTime.fromJSDate(values.sendDate).toFormat(DateTimeFormat),
            sendImmediately: values.sendMode === '1'
        });

        this.props.createNewNotification(data).then(() => {
            formik.setSubmitting(false);
            if (this.props.createNotificationResult.isSuccessful) {
                setTimeout(function () {
                    window.location = Site.resolveClientUrl('NotificationList');
                }, 3000);
            }
        });
    }

    getRecipientIds = (ids, collection) => {
        for (let i = 0; i < collection.length; i++) {
            const item = collection[i];
            if (item.children && item.children.length > 0) {
                this.getRecipientIds(ids, item.children);
            } else {
                ids.push(item.node.props.realValue);
            }
        }
    }

    schema = object().shape({
        notificationTypeId: string().required(Labels.FieldRequired),
        title: string().required(Labels.FieldRequired),
        content: string().required(Labels.FieldRequired),
        recipients: array().required(Labels.FieldRequired),
        sendMode: string(),
        sendDate: date()
            .when('sendMode', (sendMode, schema) => sendMode === '2' ?
                schema.required(Labels.FieldRequired).min(Today, field => `Send date must be later than ${field.min.toLocaleString()}`)
                : schema)
    })

    renderNotificationTypeOption = item => <option key={item.notificationTypeID} value={item.notificationTypeID} >{item.notificationTypeName}</option>

    renderNotificationTemplateOption = item => <option key={item.notificationTemplateId} value={item.notificationTemplateId} >{item.notificationTemplateName}</option>

    renderProductOption = item => <option key={item.id} value={item.id} >{item.name}</option>

    renderSendModeOption = item => <option key={item.value} value={item.value} >{item.label}</option>;

    handleSubmit = values => {
        const formatDate = values.sendDate.toLocaleString('en-US',
            { hour12: true, month: '2-digit', day: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' });

        const message = values.sendMode === '1' ? 'Do you want to send the notification to the selected recipient(s) immediately?'
            : `Do you want to send the notification to the selected recipient(s)  at ${formatDate}?`;
        this.props.toggleConfirmationPopup(true, message);
    }

    handleNotificationTypeChange = event => {
        const formik = this.refs.form;
        this.props.getRecipients(event.target.value, formik.state.values.productId);
        this.props.getTemplates(event.target.value);

        formik.setValues({
            ...formik.state.values,
            notificationTypeId: event.target.value,
            recipients: [],
            templateId: '',
            params: [],
            title: '',
            content: ''
        });
    }

    handleTemplateChange = event => {
        const formik = this.refs.form;

        if (event.target.value) {
            const selectedTemplate = this.props.notificationTemplates.find(x => x.notificationTemplateId === parseInt(event.target.value, 10));

            const paramsInTitle = selectedTemplate.title.match(/{[_-\w\s]+}/g) || [];
            const paramsInContent = selectedTemplate.content.match(/{[_-\w\s]+}/g) || [];

            const paramNames = new Set([...paramsInTitle, ...paramsInContent]);

            formik.setValues({
                ...formik.state.values,
                templateId: event.target.value,
                params: [...paramNames].map(x => ({ name: x, value: '' })),
                title: selectedTemplate.title,
                content: selectedTemplate.content,
                canEditTitleContent: false
            });
        } else {
            formik.setValues({
                ...formik.state.values,
                templateId: event.target.value,
                params: [],
                title: '',
                content: '',
                canEditTitleContent: true
            });
        }
    }

    handleProductChange = event => {
        const formik = this.refs.form;
        this.props.getRecipients(formik.state.values.notificationTypeId, event.target.value);
        formik.handleChange(event);
        if (formik.state.values.recipients.length > 0) {
            formik.setFieldValue('recipients', []);
        }
    }

    handleRecipientChange = (value, label, extra) => {
        const formik = this.refs.form;
        this.selectedNodes = extra.allCheckedNodes;
        formik.setFieldValue('recipients', value);
    }

    handleResetTitleContent = () => {
        const message = 'Reset Title and Content will recover the original template and discard your changes.';
        this.props.toggleResetConfirmationPopup(true, message);
    }

    handleConfirmResetTitleContent = () => {
        this.props.toggleResetConfirmationPopup(false);

        const formik = this.refs.form;
        const selectedTemplate = this.props.notificationTemplates.find(x => x.notificationTemplateId === parseInt(formik.state.values.templateId, 10));

        formik.setValues({
            ...formik.state.values,
            title: selectedTemplate.title,
            content: selectedTemplate.content,
            canEditTitleContent: false,
            params: formik.state.values.params.map(x => ({ name: x.name, value: '' }))
        });
    }

    handleEditTitleContent = () => {
        const message = 'You can edit the Title and Content but the system doesn\'t support manually change the text in {}';
        this.props.toggleEditConfirmationPopup(true, message);
    }

    handleConfirmEditTitleContent = () => {
        this.props.toggleEditConfirmationPopup(false);
        const formik = this.refs.form;
        formik.setFieldValue('canEditTitleContent', true);
    }

    handleReplaceParamsWithValue = () => {
        const formik = this.refs.form;
        const params = formik.state.values.params;
        const selectedTemplate = this.props.notificationTemplates.find(x => x.notificationTemplateId === parseInt(formik.state.values.templateId, 10));

        let { title, content } = selectedTemplate;

        for (let i = 0; i < params.length; i++) {
            if (params[i].value) {
                title = title.replace(new RegExp(params[i].name, 'g'), params[i].value);
                content = content.replace(new RegExp(params[i].name, 'g'), params[i].value);
            }
        }

        formik.setValues({
            ...formik.state.values,
            title,
            content
        });
    }

    handleGerneratePreview = markdown => {
        return Promise.resolve(
            <ContentPreview text={markdown} />
        );
    }

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
            <Form onSubmit={handleSubmit} className="new-notification-form">
                <FormGroup>
                    <Label for="notificationTypeId" className="form-label required">Notification Type</Label>
                    <div>
                        <Input type="select"
                            invalid={touched.notificationTypeId && errors.notificationTypeId}
                            name="notificationTypeId"
                            onChange={this.handleNotificationTypeChange}
                            value={values.notificationTypeId}
                        >
                            <option value="" disabled hidden>{Labels.PleaseSelect}</option>
                            {this.props.notificationTypes.map(this.renderNotificationTypeOption)}
                        </Input>
                        {touched.notificationTypeId && errors.notificationTypeId && <FormFeedback>{errors.notificationTypeId}</FormFeedback>}
                    </div>
                </FormGroup>
                <FormGroup>
                    <Label for="productId" className="form-label">Product</Label>
                    <div>
                        <Input type="select"
                            value={values.productId}
                            name="productId"
                            onChange={this.handleProductChange}
                        >
                            <option value="">{Labels.None}</option>
                            {this.props.products.map(this.renderProductOption)}
                        </Input>
                    </div>
                </FormGroup>
                <FormGroup>
                    <Label for="productId" className="form-label required">Recipients</Label>
                    <div>
                        <TreeSelect value={values.recipients} onChange={this.handleRecipientChange} placeholder={Labels.PleaseSelect}
                            treeData={this.props.recipientOptions} className={touched.recipients && errors.recipients && invalidFieldClass}
                            treeDefaultExpandAll
                        />
                        {touched.recipients && errors.recipients && <FormFeedback>{errors.recipients}</FormFeedback>}
                    </div>
                </FormGroup>
                <FormGroup className="row-has-reset">
                    <Label for="notificationTypeId" className="form-label">Notification Template</Label>
                    <div>
                        <Input type="select"
                            invalid={touched.templateId && errors.templateId}
                            name="templateId"
                            onChange={this.handleTemplateChange}
                            value={values.templateId}
                        >
                            <option value="">{Labels.None}</option>
                            {this.props.notificationTemplates.map(this.renderNotificationTemplateOption)}
                        </Input>
                        {touched.templateId && errors.templateId && <FormFeedback>{errors.templateId}</FormFeedback>}
                    </div>
                    {values.templateId &&
                        <div className="action">
                            <div className="circle orange" title="Edit Title & Content" onClick={this.handleEditTitleContent}  >
                                <span className="icon-edit" />
                            </div>
                            <div className="circle blue" title="Reset Title & Content" onClick={this.handleResetTitleContent}  >
                                <span className="icon-reset" />
                            </div>
                        </div>
                    }
                </FormGroup>
                <FormGroup>
                    <Label for="title" className="form-label required">Title</Label>
                    <div>
                        <Input value={values.title} name="title"
                            invalid={touched.title && errors.title}
                            onChange={handleChange} readOnly={!values.canEditTitleContent && values.templateId}
                        />
                        {touched.title && errors.title && <FormFeedback>{errors.title}</FormFeedback>}
                    </div>
                </FormGroup>
                <FormGroup>
                    <Label for="content" className="form-label required">Content</Label>
                    <div>
                        <ReactMde className={touched.content && errors.content && invalidFieldClass}
                            value={values.content} commands={editorCommandList}
                            onChange={content => setFieldValue('content', content)}
                            selectedTab={values.mode}
                            onTabChange={selectedTab => setFieldValue('mode', selectedTab)}
                            generateMarkdownPreview={this.handleGerneratePreview}
                            maxEditorHeight={400} readOnly={!values.canEditTitleContent && values.templateId}
                        />
                        {touched.content && errors.content && <FormFeedback>{errors.content}</FormFeedback>}
                    </div>
                </FormGroup>
                {values.params.length > 0 && !values.canEditTitleContent &&
                    <FormGroup>
                        <fieldset className="parameter-area">
                            <legend>Parameters</legend>
                            <FormGroup className="parameters">
                                {values.params.map((x, i) =>
                                    (
                                        <div className="parameter-col">
                                            <Label for={`params.${i}.value`} className="form-label">{x.name}</Label>
                                            <div>
                                                <Input value={x.values} name={`params.${i}.value`} disabled={values.mode === 'preview'}
                                                    onChange={handleChange} onBlur={this.handleReplaceParamsWithValue}
                                                />
                                            </div>
                                        </div>
                                    )
                                )}
                            </FormGroup>
                        </fieldset>
                    </FormGroup>
                }
                <FormGroup>
                    <Label for="sendMode" className="form-label required">Send Date/Time</Label>
                    <div>
                        <div className="send-date-area">
                            <Input type='select'
                                value={values.sendMode}
                                name="sendMode"
                                onChange={handleChange}
                            >
                                {SendModeOptions.map(this.renderSendModeOption)}
                            </Input>
                            <DatePicker selected={values.sendDate} showTimeSelect disabled={values.sendMode === '1'}
                                onChange={value => setFieldValue('sendDate', value)} minDate={Today}
                                className={touched.sendDate && errors.sendDate && invalidFieldClass}
                            />
                            <span>(GMT +8)</span>
                            {touched.sendDate && errors.sendDate && <FormFeedback>{errors.sendDate}</FormFeedback>}
                        </div>
                    </div>
                </FormGroup>
                <div className="list-buttons">
                    <Button className="btn-submit" disabled={isSubmitting} type="submit">{Labels.Submit}</Button>
                </div>
            </Form >
        );
    }

    render() {
        if (!this.props.notificationTypes || !this.props.products) {
            return null;
        }

        return (
            <>
                <NCBBreadcrum chatbotName="Skype Chatbot" pageTitle="New Notification" />
                <div className="new-notification">
                    {this.props.createNotificationResult &&
                        <ResponseMessage isSuccessful={this.props.createNotificationResult.isSuccessful}
                            message={this.props.createNotificationResult.message || Labels.CreateNotificationSuccessfully} />
                    }

                    <Formik ref="form"
                        initialValues={{
                            notificationTypeId: '',
                            templateId: '',
                            productId: '',
                            recipients: [],
                            title: '',
                            content: '',
                            sendDate: Today,
                            sendMode: '1',
                            params: [],
                            mode: 'write',
                            canEditTitleContent: false
                        }}
                        render={this.renderForm}
                        validationSchema={this.schema}
                        onSubmit={this.handleSubmit}
                    />

                </div>
                <ConfirmPopup
                    onConfirm={this.handleConfirm}
                    onToggle={this.handleToggleConfirmationPopup}
                    isOpen={this.props.confimrationPopup.isOpen}
                    content={this.props.confimrationPopup.content}
                />
                <ConfirmPopup
                    onConfirm={this.handleConfirmResetTitleContent}
                    onToggle={this.handleToggleResetConfirmationPopup}
                    isOpen={this.props.resetConfimrationPopup.isOpen}
                    content={this.props.resetConfimrationPopup.content}
                    warningIcon
                />
                <ConfirmPopup
                    onConfirm={this.handleConfirmEditTitleContent}
                    onToggle={this.handleToggleEditConfirmationPopup}
                    isOpen={this.props.editConfimrationPopup.isOpen}
                    content={this.props.editConfimrationPopup.content}
                    warningIcon
                />
            </>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    return {
        notificationTypes: state.notificationTypes,
        products: state.products,
        recipientOptions: state.recipients,
        createNotificationResult: state.createNotificationResult,
        confimrationPopup: state.confimrationPopup,
        notificationTemplates: state.notificationTemplates,
        resetConfimrationPopup: state.resetConfimrationPopup,
        editConfimrationPopup: state.editConfimrationPopup
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);