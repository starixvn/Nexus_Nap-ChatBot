import React from 'react';
import { shallow } from 'enzyme';

import OptionAll from '../OptionAll';

describe('OptionAll ', () => {
    afterAll(() => {
        jest.resetAllMocks();
    });

    describe('changeHandler', () => {
        it('should do as expected', () => {
            // Arrange
            const props = {
                language: 'en-US',
                id: 'optionAll',
                checked: false,
                onChange: jest.fn()
            };
            const checkedItem = {
                key: 'key',
                value: 'value',
                checked: false
            };
            const container = shallow(<OptionAll {...props} />).instance();

            // Act
            container.changeHandler(checkedItem);

            // Assert
            expect(props.onChange).toHaveBeenCalledWith(checkedItem.checked);
        });
    });

    describe('render', () => {
        it('should match snapshot', () => {
            // Arrange
            const props = {
                language: 'en-US',
                id: 'optionAll',
                checked: false,
                onChange: jest.fn()
            };

            // Act
            const container = shallow(<OptionAll {...props} />);

            // Assert
            expect(container.render()).toMatchSnapshot();
        });
    });
})