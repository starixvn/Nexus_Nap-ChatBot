import './AppContainer.scss';

import React from 'react';
import { connect } from 'react-redux';
import { DateTime } from 'luxon';
import { Form, FormGroup, Label, Button, Col, Input, FormFeedback, Row } from 'reactstrap';
import { stringify } from 'qs';

import { Formik, getIn } from 'formik';
import * as Yup from 'yup';
import ReactMde from 'react-mde';
import { NCBBreadcrum } from '../../../common/breadcrumbs';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../../actions/index';
import TreeSelect from '../../../common/TreeSelect/index';
import DatePicker from '../../../common/DatePicker/index';
import Labels from '../../../common/Labels';
import { DateTimeFormat, editorCommandList } from '../../../constants/constants';
import ResponseMessage from '../../../common/response-message/ResponseMessage';
import Site from '../../../lib/site';
import ConfirmPopup from '../../../common/ConfirmPopup';
import ContentPreview from './ContentPreview';
import "react-mde/lib/styles/css/react-mde-all.css";

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

    componentWillReceiveProps(nextProps) {
        if (nextProps.template !== this.props.template) {
            this.changeTemplate(nextProps);
        }
    }

    onChange = (fieldName, fieldValue) => {
        this.setState({
            [fieldName]: fieldValue,
            isChanged: true
        });
    }

    detectParams = text => {
        const paramRegex = /{[^{}]+}/g;
        const params = [];

        let copiedText = text.slice(0);
        let match = copiedText.match(paramRegex);

        while (match) {
            const matchText = match[0];
            const regex = new RegExp(matchText, 'g');

            params.push({ name: matchText, value: { value: matchText, isValid: false } });
            copiedText = copiedText.replace(regex, '');
            match = copiedText.match(paramRegex);
        }

        return params;
    }

    createMiddleTemplate = text => text.replace(/{/g, '<span class="red">{').replace(/}/g, '}</span>');

    getTemplate = () => {
        this.props.getTemplate();
    }

    changeTemplate(props) {
        const titleTemplate = props.template.title;
        const titleMiddleTemplate = this.createMiddleTemplate(titleTemplate);
        const contentTemplate = props.template.content;
        const contentMiddleTemplate = this.createMiddleTemplate(contentTemplate);
        const params = this.detectParams(titleTemplate + contentTemplate);

        this.setState(
            {
                template: {
                    title: {
                        originTemplate: titleTemplate,
                        middleTemplate: titleMiddleTemplate,
                        processedTemplate: titleMiddleTemplate
                    },
                    content: {
                        originTemplate: contentTemplate,
                        middleTemplate: contentMiddleTemplate,
                        processedTemplate: contentMiddleTemplate
                    },
                    params: params
                }
            }
        );
    }

    splitArrayIntoTwoColumns = array => {
        const newArray = [];
        const length = array.length;
        for (let i = 0; i < length; i += 2) {
            newArray.push(array.slice(i, i + 2));
        }

        return newArray;
    }

    processTemplate = (text, params) => {
        let copiedText = text.slice(0);
        params.map(item => {
            const regex = new RegExp(item.name, 'g');
            copiedText = copiedText.replace(regex, item.value.value);
        });

        return copiedText;
    }

    updateParam = (name, value) => {
        const params = this.state.template.params.slice(0);
        for (let i = 0; i < params.length; i++) {
            if (params[i].name === name) {
                params[i].value = value;
                break;
            }
        }

        return params;
    }

    updateTemplate = (name, value) => {
        const params = this.updateParam(name, value);
        const titleProcessedTemplate = this.processTemplate(this.state.template.title.middleTemplate, params);
        const titleFinalTemplate = this.processTemplate(this.state.template.title.originTemplate, params);
        const contentProcessedTemplate = this.processTemplate(this.state.template.content.middleTemplate, params);
        const contentFinalTemplate = this.processTemplate(this.state.template.content.originTemplate, params);

        this.setState(
            prevState => ({
                ...prevState,
                template: {
                    title: {
                        ...prevState.template.title,
                        processedTemplate: titleProcessedTemplate,
                        finalTemplate: titleFinalTemplate
                    },
                    content: {
                        ...prevState.template.content,
                        processedTemplate: contentProcessedTemplate,
                        finalTemplate: contentFinalTemplate
                    },
                    params: params
                }
            })
        );
    }

    editManually = () => {
        this.setState({
            manuallyEditted: true
        });
    }

    onChangeParam = (name, value) => {
        this.updateTemplate(name, value);
    }

    editTitle = value => {
        console.log(value);
    }

    editContent = value => {
        console.log(value);
    }

    handleToggleConfirmationPopup = () => {
        this.props.toggleConfirmationPopup(!this.props.confimrationPopup.isOpen);
        this.refs.form.setSubmitting(false);
    }

    handleConfirm = () => {
        this.props.toggleConfirmationPopup(false);

        const formik = this.refs.form;
        const values = formik.state.values;

        const recipientIds = [];
        this.getRecipientIds(recipientIds, this.selectedNodes);

        let title = values.title;
        let content = values.content;

        for (let i = 0; i < values.params.length; i++) {
            title = title.replace(new RegExp(values.params[i].name, 'g'), values.params[i].value);
            content = content.replace(new RegExp(values.params[i].name, 'g'), values.params[i].value);
        }

        const data = stringify({
            notificationTypeId: values.notificationTypeId,
            productId: values.productId,
            recipientIds: recipientIds.toString(),
            title: title,
            content: content,
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

    schema = Yup.object().shape({
        notificationTypeId: Yup.string().required(Labels.FieldRequired),
        title: Yup.string().required(Labels.FieldRequired),
        content: Yup.string().required(Labels.FieldRequired),
        recipients: Yup.array().required(Labels.FieldRequired),
        sendMode: Yup.string(),
        sendDate: Yup.date().min(Today, field => `Send date must be later than ${field.min.toLocaleString()}`)
            .when('sendMode', (sendMode, schema) => sendMode === '2' ? schema.required(Labels.FieldRequired) : schema),
    })

    renderNotificationTypeOption = item => <option key={item.notificationTypeID} value={item.notificationTypeID} >{item.notificationTypeName}</option>

    renderNotificationTemplateOption = item => <option key={item.notificationTemplateId} value={item.notificationTemplateId} >{item.notificationTemplateName}</option>

    renderProductOption = item => <option key={item.id} value={item.id} >{item.name}</option>

    renderSendModeOption = item => <option key={item.value} value={item.value} >{item.label}</option>;

    handleSubmit = values => {
        const formatDate = values.sendDate.toLocaleString('en-US',
            { hour12: true, month: '2-digit', day: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' });

        const message = values.sendMode === '1' ? 'Do you want to send immediately the notification to the selected recipient?'
            : `Do you want to send the notification at ${formatDate} to the selected recipient?`;
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
                content: selectedTemplate.content
            });
        } else {
            formik.setValues({
                ...formik.state.values,
                templateId: event.target.value,
                params: [],
                title: '',
                content: ''
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
        const formik = this.refs.form;
        const selectedTemplate = this.props.notificationTemplates.find(x => x.notificationTemplateId === parseInt(formik.state.values.templateId, 10));

        formik.setValues({
            ...formik.state.values,
            title: selectedTemplate.title,
            content: selectedTemplate.content
        });
    }

    handleGerneratePreview = markdown => {
        const params = this.refs.form.state.values.params;

        for (let i = 0; i < params.length; i++) {
            markdown = markdown.replace(new RegExp(params[i].name, 'g'), params[i].value);
        }

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
                <FormGroup row>
                    <Label for="notificationTypeId" className="form-label required" sm={2}>Notification Type</Label>
                    <Col sm={10}>
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
                    </Col>
                </FormGroup>
                <FormGroup row>
                    <Label for="productId" className="form-label" sm={2}>Product</Label>
                    <Col sm={10}>
                        <Input type="select"
                            value={values.productId}
                            name="productId"
                            onChange={this.handleProductChange}
                        >
                            <option value="">{Labels.None}</option>
                            {this.props.products.map(this.renderProductOption)}
                        </Input>
                    </Col>
                </FormGroup>
                <FormGroup row>
                    <Label for="productId" className="form-label required" sm={2}>Recipients</Label>
                    <Col sm={10}>
                        <TreeSelect value={values.recipients} onChange={this.handleRecipientChange} placeholder={Labels.PleaseSelect}
                            treeData={this.props.recipientOptions} className={touched.recipients && errors.recipients && 'is-invalid'}
                            treeDefaultExpandAll
                        />
                        {touched.recipients && errors.recipients && <FormFeedback>{errors.recipients}</FormFeedback>}
                    </Col>
                </FormGroup>
                <FormGroup row>
                    <Label for="notificationTypeId" className="form-label" sm={2}>Template</Label>
                    <Col sm={4}>
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
                    </Col>
                    <Col sm={6}>
                        <Button type="button" onClick={this.handleResetTitleContent} disabled={!values.templateId}>Reset Title & Content</Button>
                    </Col>
                </FormGroup>
                <FormGroup row>
                    <Label for="title" className="form-label required" sm={2}>Title</Label>
                    <Col sm={10}>
                        <Input value={values.title} name="title"
                            invalid={touched.title && errors.title}
                            onChange={handleChange}
                        />
                        {touched.title && errors.title && <FormFeedback>{errors.title}</FormFeedback>}
                    </Col>
                </FormGroup>
                <FormGroup row>
                    <Label for="content" className="form-label required" sm={2}>Content</Label>
                    <Col sm={10}>
                        <ReactMde className={touched.content && errors.content && 'is-invalid'}
                            value={values.content} commands={editorCommandList}
                            onChange={content => setFieldValue('content', content)}
                            selectedTab={values.mode}
                            onTabChange={selectedTab => setFieldValue('mode', selectedTab)}
                            generateMarkdownPreview={this.handleGerneratePreview}
                            maxEditorHeight={400}
                        />
                        {touched.content && errors.content && <FormFeedback>{errors.content}</FormFeedback>}
                    </Col>
                </FormGroup>
                {values.params.length > 0 &&
                    <Row >
                        <Col sm={{ size: 10, offset: 2 }}>
                            <fieldset>
                                <legend>Parameters</legend>
                                <FormGroup row>
                                    {values.params.map((x, i) =>
                                        <>
                                            <Label for={`params.${i}.value`} className="form-label" sm={2}>{x.name}</Label>
                                            <Col sm={4}>
                                                <Input value={x.values} name={`params.${i}.value`} disabled={values.mode === 'preview'}
                                                    invalid={getIn(touched, `params.${i}.value`) && getIn(errors, `params.${i}.value`)}
                                                    onChange={handleChange}
                                                />
                                            </Col>
                                        </>
                                    )}
                                </FormGroup>
                            </fieldset>
                        </Col>
                    </Row>
                }
                <FormGroup row>
                    <Label for="sendMode" className="form-label required" sm={2}>Send Date/Time</Label>
                    <Col sm={10} >
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
                                className={touched.sendDate && errors.sendDate && 'is-invalid'}
                            />
                            <span>(GMT +8)</span>
                            {touched.sendDate && errors.sendDate && <FormFeedback>{errors.sendDate}</FormFeedback>}
                        </div>
                    </Col>
                </FormGroup>
                <FormGroup row className="list-buttons">
                    <Button className="btn-submit offset-sm-2" disabled={isSubmitting} type="submit">Save</Button>
                </FormGroup>
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
                            mode: 'write'
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
        notificationTemplates: state.notificationTemplates
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);