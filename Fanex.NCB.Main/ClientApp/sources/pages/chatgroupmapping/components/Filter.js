import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Form, FormGroup, Label } from 'reactstrap';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions';
import MultiselectTreeView from '../../common/react-multiselect-treeview/MultiselectTreeView';
import Labels from '../../common/Labels';
import AutoComplete from '../../common/auto-complete';
import Site from '../../lib/site';
import { PAGING_SIZES } from '../../constants/constants';

export class Filter extends Component {
    static defaultProps = {
        settings: {
            bots: [],
            recipientTypes: [],
            mappingStatuses: [],
            licenseeStatuses: []
        },
        getSettings: function () {
            // do nothing
        },
        filterRecipients: function () {
            // do nothing
        }
    };

    constructor(props) {
        super(props);

        this.state = {
            channelIds: '',
            botIds: '',
            recipientTypeIds: '',
            recipient: {
                value: ''
            },
            groupName: {
                value: ''
            },
            licenseeStatusIds: '',
            mappingStatusIds: '',
            settings: {
                bots: [],
                recipientTypes: [],
                mappingStatuses: [],
                licenseeStatuses: []
            }
        };
    }

    componentDidMount() {
        this.props.getSettings();
        this.props.filterRecipients([], 0);
    }

    static getDerivedStateFromProps(nextProps, prevState) {
        if (prevState.settings.bots.length === 0 && nextProps.settings.bots) {
            const selectedRecipientTypeIds = [];
            const recipientTypes = nextProps.settings.recipientTypes.map(item => {
                selectedRecipientTypeIds.push(item['id']);
                return { id: item.id, text: item.name, checked: true, isLeaf: true };
            });

            const selectedMappingStatusIds = [];
            const mappingStatuses = nextProps.settings.mappingStatuses.map(item => {
                selectedMappingStatusIds.push(item['id']);
                return { id: item.id, text: item.name, checked: true, isLeaf: true };
            });

            const selectedlicenseeStatusIds = [];
            const licenseeStatuses = nextProps.settings.licenseeStatuses.map(item => {
                selectedlicenseeStatusIds.push(item.id);
                return { id: item.id, text: item.name, checked: true, isLeaf: true };
            });

            const state = {
                channelIds: nextProps.settings.channelIds,
                botIds: nextProps.settings.botIds,
                recipientTypeIds: selectedRecipientTypeIds.join(','),
                recipient: {
                    value: ''
                },
                licenseeStatusIds: selectedlicenseeStatusIds.join(','),
                mappingStatusIds: selectedMappingStatusIds.join(','),
                settings: {
                    channels: nextProps.settings.channels,
                    bots: nextProps.settings.bots,
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
            channelIds: this.state.channelIds,
            botIds: this.state.botIds,
            recipientTypeIds: this.state.recipientTypeIds,
            recipientName: this.state.recipient.value,
            groupName: this.state.groupName.value,
            mappingStatusIds: this.state.mappingStatusIds,
            licenseeStatusIds: this.state.licenseeStatusIds,
            pageIndex: 1,
            pageSize: PAGING_SIZES[0]
        };
    }

    checkValidParams = () => {
        const { channelIds, botIds, recipientTypeIds, mappingStatusIds, licenseeStatusIds } = this.state;
        return channelIds != '' && botIds !== ''
            && recipientTypeIds !== '' && mappingStatusIds !== '' && licenseeStatusIds !== '';
    }

    submitHandler = e => {
        e.preventDefault();
        const params = this.getParams();
        this.props.setGridParams(params);
        this.props.getGridData(params);
    }

    render() {
        return (
            <Form className="form-filter-new" tag='div'>
                <div className="form-row">
                    <FormGroup>
                        <Label className="form-label">{Labels.RecipientType}</Label>
                        <MultiselectTreeView
                            data={this.state.settings.recipientTypes} isSelectedAll
                            onChange={selectedItems => this.onChange('recipientTypeIds', selectedItems.map(item => item.id).join(','))}
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
                    <FormGroup className="licensee-status-group">
                        <Label className="form-label">{Labels.LicenseeStatus}</Label>
                        <MultiselectTreeView
                            data={this.state.settings.licenseeStatuses} isSelectedAll
                            onChange={selectedItems => this.onChange('licenseeStatusIds', selectedItems.map(item => item.id).join(','))}
                        />
                    </FormGroup>
                    <FormGroup className="mapping-status-group">
                        <Label className="form-label">{Labels.MappingStatus}</Label>
                        <MultiselectTreeView
                            data={this.state.settings.mappingStatuses} isSelectedAll
                            onChange={selectedItems => this.onChange('mappingStatusIds', selectedItems.map(item => item.id).join(','))}
                        />
                    </FormGroup>
                    <FormGroup className="bot-service-group">
                        <Label className="form-label">{Labels.BotService}</Label>
                        <MultiselectTreeView
                            data={this.state.settings.channels} isSelectedAll
                            onChange={selectedItems => this.onChange('channelIds', selectedItems.map(item => item.id).join(','))}
                        />
                    </FormGroup>
                    <FormGroup>
                        <Label className="form-label">{Labels.BotName}</Label>
                        <MultiselectTreeView
                            data={this.state.settings.bots} isSelectedAll
                            onChange={selectedItems => this.onChange('botIds', selectedItems.map(item => item.id).join(','))}
                        />
                    </FormGroup>
                    <FormGroup>
                        <button type="submit" id="submit-button" disabled={!this.checkValidParams()}
                            className="btn-submit" onClick={this.submitHandler}
                        >{Labels.Submit}</button>
                    </FormGroup>
                </div>
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