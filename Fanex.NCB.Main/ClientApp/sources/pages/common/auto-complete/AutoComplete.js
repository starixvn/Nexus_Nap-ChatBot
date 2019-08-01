import React, { Component } from 'react';
import PropTypes from 'prop-types';
import axios from 'axios';
import Overlay from './Overlay';
import SuggestionList from './SuggestionList';
import Timer from './timer';
import './AutoComplete.scss';

const DELAY_TIME_MILLISECOND = 300;

export default class AutoComplete extends Component {
    static propTypes = {
        cache: PropTypes.bool,
        htmlAttributes: PropTypes.object,
        onInput: PropTypes.func,
        onSelect: PropTypes.func,
        queryParamName: PropTypes.string,
        responseDataField: PropTypes.string,
        url: PropTypes.string
    }

    static defaultProps = {
        url: null,
        cache: true,
        queryParamName: 'query'
    }

    constructor(props) {
        super(props);

        this.state = {
            suggestions: null,
            selectedIndex: -1
        };

        this.input = null;
        this.cacheData = {};

        this.onChange = Timer.debounce(this._onChange, DELAY_TIME_MILLISECOND).bind(this);
    }

    get isShowSuggestionList() {
        return this.state.suggestions && this.state.suggestions.length > 0;
    }

    _onKeyDown = event => {
        if (!this.isShowSuggestionList) {
            return;
        }

        const selectedIndex = this.state.selectedIndex;
        switch (event.keyCode) {
            case 38: //up
                if (selectedIndex > 0) {
                    this.setState({ selectedIndex: selectedIndex - 1 });
                }
                break;
            case 40: //down
                if (selectedIndex < this.state.suggestions.length - 1) {
                    this.setState({ selectedIndex: selectedIndex + 1 });
                }
                break;
            case 13: //enter
                if (selectedIndex > -1) {
                    const selectedItem = this.state.suggestions[selectedIndex];
                    this._onSelect(selectedItem);
                }
                break;
        }
    }

    _setStateWithCaching(value, suggestionList) {
        if (this.props.cache) {
            const cacheKey = value.toLowerCase();
            this.cacheData[cacheKey] = suggestionList;
        }

        this.setState({
            suggestions: suggestionList
        });
    }

    _clearState = () => {
        this.setState({ suggestions: null, selectedIndex: -1 });
    }

    _onSelect = selectedItem => {
        const { onSelect } = this.props;

        this.input.value = typeof (selectedItem) === 'object' ? selectedItem.name : selectedItem;
        onSelect && onSelect(this.input.value);

        this._clearState();
    }

    _getDataByUrl(url, value) {
        const { queryParamName } = this.props;
        const apiUrl = `${url}?${queryParamName}=${value}`;

        axios.get(apiUrl, {
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        }).then(response => {
            this._setStateWithCaching(value, response.data);
        });
    }

    _onChange = () => {
        const value = (this.input.value || '').trim();

        const { onInput } = this.props;
        onInput && onInput(value);

        if (value.length < 3) {
            this._clearState();
            return;
        }

        const cachedData = this.cacheData[value];
        if (cachedData) {
            this.setState({ suggestions: cachedData });
            return;
        }

        const { url } = this.props;
        if (url) {
            this._getDataByUrl(url, value);
        }
    }

    render() {
        return (
            <div className="ac-container">
                <input type="text" className="input-text form-control" {...this.props.htmlAttributes}
                    onChange={this.onChange} onKeyDown={this._onKeyDown}
                    ref={element => this.input = element}
                    autoFocus
                />

                {this.isShowSuggestionList &&
                    <Overlay isOpening onClose={this._clearState}>
                        <SuggestionList selectedIndex={this.state.selectedIndex}
                            width={this.input.clientWidth}
                            dataList={this.state.suggestions}
                            onSelect={this._onSelect}
                        />
                    </Overlay>}
            </div>
        );
    }
}