import React from 'react';
import { connect } from 'react-redux';
import { Form, FormGroup, Label, Button } from 'reactstrap';
import { Container, Row, Col } from 'reactstrap';
import { NCBBreadcrum } from '../../../common/breadcrumbs';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../../actions/index';
import { SelectField } from '../../../common/SelectField';
import { TextField } from '../../../common/text-field/TextField';
import { InputField } from '../../../common/InputField';
import { TemplateTextField } from '../../../common/template-text-field/TemplateTextField';

import './AppContainer.scss';

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
        this.props.getSettings();
        this.props.getTemplateTypes();
    }

    componentWillReceiveProps(nextProps) {
        if(nextProps.template !== this.props.template)  {
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

    createMiddleTemplate = text=> text.replace(/{/g, '<span class="red">{').replace(/}/g, '}</span>');

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

        return newArray
    }

    processTemplate = (text, params) => {
        let copiedText = text.slice(0);
        params.map(item => {
            let regex = new RegExp(item.name, 'g');
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
        console.log(value)
    }

    editContent = value => {
        console.log(value)
    }

    render() {
        if (!this.props.settingData || !this.props.notificationTemplates) {
            return null;
        }

        const splittedParams = this.splitArrayIntoTwoColumns(this.state.template.params);
        
        return (
            <div>
                <NCBBreadcrum chatbotName="Skype Chatbot" pageTitle="New Notification" />
                <div className="content form new-notification-form">
                    <Form>
                        <FormGroup>
                            <Label for="notificationType" className="form-label required">Notification Type</Label>
                            <SelectField
                                data={this.props.settingData.notificationTypes}
                                isSummited={this.state.isSummited}
                                onChange={notificationTypeId => { this.onChange('notificationTypeId', notificationTypeId); }}
                            />
                        </FormGroup>
                        <FormGroup>
                            <Label for="notificationType" className="form-label">Product</Label>
                            <SelectField
                                data={this.props.settingData.products}
                                isSummited={this.state.isSummited}
                                onChange={notificationTypeId => { this.onChange('notificationTypeId', notificationTypeId); }}
                            />
                        </FormGroup>
                        <FormGroup>
                            <Label for="notificationType" className="form-label">Notification Template</Label>
                            <SelectField
                                data={this.props.notificationTemplates}
                                isSummited={this.state.isSummited}
                                onChange={notificationTypeId => { this.getTemplate(notificationTypeId); this.onChange('notificationTypeId', notificationTypeId); }}
                            />
                        </FormGroup>
                        <FormGroup>
                            <Label for="notificationType" className="form-label required">Title
                                {
                                    !this.state.manuallyEditted &&
                                    <span className="icon-edit" onClick={() => !this.state.manuallyEditted && this.editManually()} />
                                }
                            </Label>                      
                            <div className="template-title-div">
                                {
                                    !this.state.manuallyEditted &&
                                    <TemplateTextField
                                        value={this.state.template.title.processedTemplate}
                                    />
                                }
                                {
                                    this.state.manuallyEditted &&
                                    <InputField initialValue={this.state.template.title.finalTemplate}
                                        isSummited={this.state.isSummited}
                                        onChange={value => this.editTitle(value)}
                                    />  
                                }
                            </div>
                        </FormGroup>
                        <FormGroup>
                            <Label for="notificationType" className="form-label required">Content
                                {
                                    !this.state.manuallyEditted &&
                                    <span className="icon-edit" onClick={() => !this.state.manuallyEditted && this.editManually()} />
                                }
                            </Label>
                            <div className="template-content-div">
                                {
                                    !this.state.manuallyEditted &&
                                    <TemplateTextField
                                        value={this.state.template.content.processedTemplate}
                                    />
                                }
                                {
                                    this.state.manuallyEditted &&
                                    <TextField initialValue={this.state.template.content.finalTemplate}
                                        isSummited={this.state.isSummited}
                                        onChange={value => this.onChange("remark", value)}
                                        required                                      
                                    />
                                }
                            </div>
                        </FormGroup>
                        <FormGroup>
                            <Label for="notificationType" className="form-label">&nbsp;</Label>
                            {
                                !this.state.manuallyEditted &&
                                <div className="form-params">
                                    <div className="title-params">Parameters</div>
                                    <Container>
                                        {
                                            splittedParams.map((items, rowIndex) => {
                                                return <Row key={rowIndex} className="row-params">
                                                    {
                                                        items.map((item, colIndex) => {
                                                            return <Col key={colIndex} className="col-params">
                                                                <Label className="form-label">{item.name}</Label>
                                                                <InputField initialValue={item.value.value}
                                                                    isSummited={this.state.isSummited}
                                                                    onChange={(value) => this.onChangeParam(item.name, value)}
                                                                />
                                                            </Col>;
                                                        })
                                                    }
                                                </Row>
                                            })
                                        }
                                    </Container>
                                </div>
                            }
                        </FormGroup>
                        <FormGroup>
                            <Label for="notificationType" className="form-label required">Send Date/Time</Label>
                        </FormGroup>
                        <Button className="btn-submit">Save</Button>
                        <Button className="btn-cancel">Cancel</Button>
                    </Form>
                </div>
            </div>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    return {
        settingData: state.settingData,
        notificationTemplates: state.notificationTemplates,
        template: state.template
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);