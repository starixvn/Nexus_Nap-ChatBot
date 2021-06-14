import React from 'react';
import { shallow } from 'enzyme';

import MultipleSelectOptionList from '../MultipleSelectOptionList';
import Option from '../Option';

describe('MultipleSelectOptionList ', () => {
    afterAll(() => {
        jest.resetAllMocks();
    });

    describe('_renderOptionList', () => {
        it('should return no information when dataSource is invalid', () => {
            // Arrange
            const props = {
                dataSource: [],
                id: 1,
                onChange: jest.fn()
            };
            const expected = <span>No information</span>;
            const container = shallow(<MultipleSelectOptionList {...props} />).instance();

            // Act
            const actual = container._renderOptionList();

            // Assert
            expect(actual).toEqual(expected);
        });

        it('should return expected Option components', () => {
            // Arrange
            const props = {
                dataSource: [
                    { key: 'key1', value: 'Value 1', checked: true, visible: true },
                    { key: 'key2', value: 'Value 2', checked: false, visible: false }
                ],
                id: 1,
                onChange: jest.fn()
            };
            const expected = [
                <Option
                    id={`1-optionItem-0`}
                    key={`1optionItem0`}
                    itemData={props.dataSource[0]}
                    onChange={props.onChange}
                />, false
            ];
            const container = shallow(<MultipleSelectOptionList {...props} />).instance();

            // Act
            const actual = container._renderOptionList();

            // Assert
            expect(actual).toEqual(expected);
        });
    });

    describe('render', () => {
        it('should return expected component', () => {
            // Arrange
            const props = {
                dataSource: [],
                id: 1,
                onChange: jest.fn()
            };
            const container = shallow(<MultipleSelectOptionList {...props} />).instance();
            container._renderOptionList = jest.fn(() => <div />);

            // Act
            const actual = container.render();

            // Assert
            expect(actual).toEqual(
                <ul className="multiple-select-options">
                    <div />
                </ul>
            );
        });
    });
});