import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Form, FormGroup, Label } from 'reactstrap';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions';
import { MultipleSelectField } from '../../common/multiple-select-field/MultipleSelectField';
import Labels from '../../common/Labels';
import AutoComplete from '../../common/auto-complete';
import Site from '../../lib/site';
import { PAGING_SIZES } from '../../constants/constants';

export class Filter extends Component {
    constructor(props) {
        super(props);

        this.state = {
            botIds: {
                value: ''
            },
            recipientTypes: {
                value: ''
            },
            recipient: {
                value: ''
            },
            groupName: {
                value: ''
            },
            licenseeStatuses: {
                value: ''
            },
            mappingStatuses: {
                value: ''
            },
            settings: {
                bots: [],
                recipientTypes: [],
                mappingStatuses: [],
                licenseeStatuses: []
            }
        };

        this.props.getSettings();
        this.props.filterRecipients([], 0);
    }

    static getDerivedStateFromProps(nextProps, prevState) {
        if (prevState.settings.bots.length === 0 && nextProps.settings.bots) {
            const selectedbotIds = [];
            const bots = nextProps.settings.bots.map(item => {
                selectedbotIds.push(item['id']);
                return { ...item, checked: true };
            });

            const selectedRecipientTypes = [];
            const recipientTypes = nextProps.settings.recipientTypes.map(item => {
                selectedRecipientTypes.push(item['id']);
                return { ...item, checked: true };
            });

            const selectedMappingStatuses = [];
            const mappingStatuses = nextProps.settings.mappingStatuses.map(item => {
                selectedMappingStatuses.push(item['id']);
                return { ...item, checked: true };
            });

            const selectedlicenseeStatuses = [];
            const licenseeStatuses = nextProps.settings.licenseeStatuses.map(item => {
                selectedlicenseeStatuses.push(item.id);
                return { ...item, checked: true };
            });

            const state = {
                botIds: {
                    value: selectedbotIds.join(',')
                },
                recipientTypes: {
                    value: selectedRecipientTypes.join(',')
                },
                recipient: {
                    value: ''
                },
                licenseeStatuses: {
                    value: selectedlicenseeStatuses.join(',')
                },
                mappingStatuses: {
                    value: selectedMappingStatuses.join(',')
                },
                settings: {
                    bots: bots,
                    recipientTypes: recipientTypes,
                    mappingStatuses: mappingStatuses,
                    licenseeStatuses: licenseeStatuses
                }
            };

            return state;
        }

        return null;
    }

    onChange = (fieldName, fieldValue) => {
        this.setState({
            [fieldName]: fieldValue
        });
    }

    getParams = () => {
        return {
            botIds: this.state.botIds.value,
            recipientTypeIds: this.state.recipientTypes.value,
            recipientName: this.state.recipient.value,
            groupName: this.state.groupName.value,
            mappingStatusIds: this.state.mappingStatuses.value,
            licenseeStatusIds: this.state.licenseeStatuses.value,
            pageIndex: 1,
            pageSize: PAGING_SIZES[0]
        };
    }

    checkValidParams = () => this.state.botIds.value !== '' &&
        this.state.recipientTypes.value !== '' &&
        this.state.mappingStatuses.value !== '' &&
        this.state.licenseeStatuses.value !== '';

    submitHandler = e => {
        e.preventDefault();
        const params = this.getParams();
        this.props.setGridParams(params);
        this.props.getGridData(params);
    }

    render() {
        return (
            <Form className="form-filter" tag='div' >
                <FormGroup>
                    <Label className="form-label">{Labels.RecipientType}</Label>
                    <MultipleSelectField
                        id={'recipientTypes'}
                        width={150}
                        initialValue={this.state.recipientTypes}
                        data={this.state.settings.recipientTypes}
                        keyField={'id'}
                        valueField={'name'}
                        statusField={'checked'}
                        isSummited={false}
                        texts={{ SelectOptions: Labels.PleaseSelect, All: Labels.All, SelectedItemCount: '# selected' }}
                        isAllTextShown
                        onChange={value => this.onChange('recipientTypes', value)}
                    />
                </FormGroup>
                <FormGroup>
                    <Label className="form-label">{Labels.Recipient}</Label>
                    <AutoComplete
                        url={Site.resolveClientUrl('ChatGroupMapping/SearchRecipient')}
                        onSelect={value => this.onChange('recipient', { value: value })}
                        onInput={value => this.onChange('recipient', { value: value })}
                        htmlAttributes={{ maxLength: 50 }}
                    />
                </FormGroup>
                <FormGroup>
                    <Label className="form-label">{Labels.GroupName}</Label>
                    <AutoComplete
                        url={Site.resolveClientUrl('ChatGroupMapping/SearchChatGroup')}
                        onSelect={value => this.onChange('groupName', { value: value })}
                        onInput={value => this.onChange('groupName', { value: value })}
                        htmlAttributes={{ maxLength: 50 }}
                    />
                </FormGroup>
                <FormGroup>
                    <Label className="form-label">{Labels.LicenseeStatus}</Label>
                    <MultipleSelectField
                        id={'licenseeStatuses'}
                        width={150}
                        initialValue={this.state.licenseeStatuses}
                        data={this.state.settings.licenseeStatuses}
                        keyField={'id'}
                        valueField={'name'}
                        statusField={'checked'}
                        isSummited={false}
                        texts={{ SelectOptions: Labels.PleaseSelect, All: Labels.All, SelectedItemCount: '# selected' }}
                        isAllTextShown
                        onChange={value => this.onChange('licenseeStatuses', value)}
                    />
                </FormGroup>
                <FormGroup>
                    <Label className="form-label">{Labels.MappingStatus}</Label>
                    <MultipleSelectField
                        id={'mappingStatuses'}
                        width={150}
                        initialValue={this.state.mappingStatuses}
                        data={this.state.settings.mappingStatuses}
                        keyField={'id'}
                        valueField={'name'}
                        statusField={'checked'}
                        isSummited={false}
                        texts={{ SelectOptions: Labels.PleaseSelect, All: Labels.All, SelectedItemCount: '# selected' }}
                        isAllTextShown
                        onChange={value => this.onChange('mappingStatuses', value)}
                    />
                </FormGroup>
                <FormGroup>
                    <Label className="form-label">{Labels.BotName}</Label>
                    <MultipleSelectField
                        id={'botIds'}
                        width={150}
                        initialValue={this.state.botIds}
                        data={this.state.settings.bots}
                        keyField={'id'}
                        valueField={'name'}
                        statusField={'checked'}
                        isSummited={false}
                        texts={{ SelectOptions: Labels.PleaseSelect, All: Labels.All, SelectedItemCount: '# selected' }}
                        isAllTextShown
                        onChange={value => this.onChange('botIds', value)}
                    />
                </FormGroup>
                <button type="submit" id="submit-button" disabled={!this.checkValidParams()} className="btn-submit" onClick={this.submitHandler}>{Labels.Submit}</button>
            </Form>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { settings } = state;
    return {
        settings
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(Filter);