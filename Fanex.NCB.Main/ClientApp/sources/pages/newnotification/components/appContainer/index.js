import React from 'react';
import { connect } from 'react-redux';
import { Form, FormGroup, Label, Button, Container, Row, Col, Input, FormFeedback } from 'reactstrap';
import { Formik } from 'formik';
import * as Yup from 'yup';
import { NCBBreadcrum } from '../../../common/breadcrumbs';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../../actions/index';
import { SelectField } from '../../../common/SelectField';
import { TextField } from '../../../common/text-field/TextField';
import { InputField } from '../../../common/InputField';
import DatePicker from '../../../common/DatePicker/index';
import { TemplateTextField } from '../../../common/template-text-field/TemplateTextField';
import Labels from '../../../common/Labels';

import './AppContainer.scss';

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

    schema = Yup.object().shape({
        notificationTypeId: Yup.string().required(Labels.FieldRequired),
        title: Yup.string().required(Labels.FieldRequired),
        content: Yup.string().required(Labels.FieldRequired)
    })

    renderNotificationTypeOption = (item) => <option key={item.notificationTypeID} value={item.notificationTypeID} >{item.notificationTypeName}</option>

    renderProductOption = (item) => <option key={item.id} value={item.id} >{item.name}</option>

    renderSendModeOption = item => <option key={item.value} value={item.value} >{item.label}</option>

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
                <FormGroup>
                    <Label for="notificationTypeId" className="form-label required">Notification Type</Label>
                    <Input type="select"
                        invalid={touched.notificationTypeId && errors.notificationTypeId}
                        name="notificationTypeId"
                        onChange={handleChange}
                        value={values.notificationTypeId}
                    >
                        <option value="" />
                        {this.props.notificationTypes.map(this.renderNotificationTypeOption)}
                    </Input>
                    {touched.notificationTypeId && errors.notificationTypeId && <FormFeedback>{errors.notificationTypeId}</FormFeedback>}
                </FormGroup>
                <FormGroup>
                    <Label for="productId" className="form-label">Product</Label>
                    <Input type="select"
                        value={values.productId}
                        name="productId"
                        onChange={handleChange}
                    >
                        <option value="">{Labels.None}</option>
                        {this.props.products.map(this.renderProductOption)}
                    </Input>
                </FormGroup>
                <FormGroup>
                    <Label for="title" className="form-label required">Title</Label>
                    <Input value={values.title} name="title"
                        invalid={touched.title && errors.title}
                        onChange={handleChange}
                    />
                    {touched.title && errors.title && <FormFeedback>{errors.title}</FormFeedback>}
                </FormGroup>
                <FormGroup>
                    <Label for="content" className="form-label required">Content</Label>
                    <Input type="textarea" value={values.content} name="content"
                        onChange={handleChange} invalid={touched.content && errors.content}
                    />
                    {touched.content && errors.content && <FormFeedback>{errors.content}</FormFeedback>}
                </FormGroup>
                <FormGroup>
                    <Label for="sendMode" className="form-label required">Send Date/Time</Label>
                    <div>
                        <Input type='select'
                            value={values.sendMode}
                            name="sendMode"
                            onChange={handleChange}
                        >
                            {SendModeOptions.map(this.renderSendModeOption)}
                        </Input>
                        <DatePicker selected={values.sendDate} showTimeSelect disabled={values.sendMode === '1'}
                            onChange={value => setFieldValue('sendDate', value)}
                        />
                        <span>(GMT +8)</span>
                    </div>

                </FormGroup>

                <Button className="btn-submit" disabled={isSubmitting}>Save</Button>
            </Form >
        );
    }

    render() {
        if (!this.props.notificationTypes || !this.props.products) {
            return null;
        }

        return (
            <div>
                <NCBBreadcrum chatbotName="Skype Chatbot" pageTitle="New Notification" />
                <div className="content form new-notification-form">
                    <Formik
                        initialValues={{
                            notificationTypeId: '',
                            productId: '',
                            title: '',
                            content: '',
                            sendDate: new Date(),
                            sendMode: '1'
                        }}
                        render={this.renderForm}
                        validationSchema={this.schema}
                        onSubmit={this.handleSubmit}
                    />

                </div>
            </div>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    return {
        notificationTypes: state.notificationTypes,
        products: state.products
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);